namespace NugetVisualizer.Core.Github
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Xml;
    using System.Xml.Linq;

    using Microsoft.Extensions.Configuration;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Exceptions;
    using NugetVisualizer.Core.PackageParser;

    using Octokit;
    using Octokit.Internal;
    using Polly;

    public class GithubPackageReader : IPackageReader
    {
        private IConfigurationRoot _configurationRoot;

        private GitHubClient _gitHubClient;
        private int _searchApiCalls;
        private object lockObject = new object();
        private System.Timers.Timer _timer;
        private AutoResetEvent _safeToCallSearchApi;

        private delegate Task<string[]> GetFiles(IProjectIdentifier projectIdentifier);

        public GithubPackageReader(IConfigurationHelper configurationHelper)
        {
            _configurationRoot = configurationHelper.GetConfiguration();
            var githubToken = _configurationRoot["GithubToken"];
            InMemoryCredentialStore credentials = new InMemoryCredentialStore(new Credentials(githubToken));
            _gitHubClient = new GitHubClient(new ProductHeaderValue(_configurationRoot["GithubOrganization"]), credentials);
            _searchApiCalls = 0;
            var timer = new System.Timers.Timer(1000*62);
            _timer = timer;
            _timer.AutoReset = false;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = false;
            _safeToCallSearchApi = new AutoResetEvent(false);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("Github api 1 minute passed, allowing more calls..");
            _searchApiCalls = 0;
            _safeToCallSearchApi.Set();
        }

        public async Task<List<IPackageContainer>> GetPackagesContentsAsync(IProjectIdentifier projectIdentifier)
        {
            var ret = new List<IPackageContainer>();
            try
            {
                await ProcessPackagesFiles(projectIdentifier, GetNetFrameworkPackagesFiles, PackageType.NetFramework, ret);
                await ProcessPackagesFiles(projectIdentifier, GetNetCore2PackagesFiles, PackageType.NetCore2, ret);
            }
            catch (RateLimitExceededException rateLimitExceededException)
            {
                throw new CannotGetPackagesContentsException("Git API returned rate limit exceeded", rateLimitExceededException);
            }
            
            return ret;
        }

        private async Task ProcessPackagesFiles(IProjectIdentifier projectIdentifier, GetFiles getFilesDelegate, PackageType packageType, List<IPackageContainer> ret)
        {
            Stack<string> packagesToProcess = null;
            
            if (!_timer.Enabled)
            {
                Debug.WriteLine("Starting timer on first call");
                _timer.Start();
            }

            var waitAndRetryForever = Policy.Handle<RateLimitExceededException>()
                .WaitAndRetryForeverAsync((retryAttempt, exception, context) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timespan, context) =>
                    {
                        Debug.WriteLine($"Got {exception.GetType()} exception");
                        // print something on the UI so that user knows it hasn't hang
                        return Task.CompletedTask;
                    });

            await waitAndRetryForever.ExecuteAsync(async () => {
                
                if (packagesToProcess == null)
                {
                    packagesToProcess = new Stack<string>(await getFilesDelegate(projectIdentifier));
                }

                packagesToProcess.TryPeek(out var packagesFile);

                while (packagesFile != null)
                {
                    if (!_timer.Enabled)
                    {
                        Debug.WriteLine("Starting timer after waiting");
                        _timer.Start();
                    }
                    var downloadedFile = (await _gitHubClient.Repository.Content.GetAllContents(
                                              _configurationRoot["GithubOrganization"],
                                              projectIdentifier.RepositoryName,
                                              packagesFile)).Single();

                    var downloadedFileContent = GetDownloadedFileContent(downloadedFile);
                    try
                    {
                        XDocument xml = XDocument.Parse(downloadedFileContent);
                        AddContainerToResult(packageType, ret, xml);
                    }
                    catch (XmlException e)
                    {
                        // ToDo : Log this in a better way
                        Trace.WriteLine($"Error {e.Message} while parsing {packagesFile} for {projectIdentifier.SolutionName}");
                    }

                    _searchApiCalls++;
                    packagesToProcess.Pop();

                    // Github's search API has a custom limit of 30 requests per minute, so we have to throttle otherwise we get kicked out. https://developer.github.com/v3/search/#rate-limit 
                    if (_searchApiCalls == 29)
                    {
                        Debug.WriteLine("Waiting for github api, 30 calls per minute..");
                        _safeToCallSearchApi.WaitOne();
                        Debug.WriteLine("Finished waiting for github api.");
                        _searchApiCalls++;
                    }
                    packagesToProcess.TryPeek(out packagesFile);
                }
            });
        }

        private static void AddContainerToResult(PackageType packageType, List<IPackageContainer> ret, XDocument xml)
        {
            switch (packageType)
            {
                case PackageType.NetFramework:
                    ret.Add(new NetFrameworkPackageContainer(xml));
                    break;
                case PackageType.NetCore2:
                    ret.Add(new NetCore2PackageContainer(xml));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(packageType), packageType, null);
            }
        }

        private async Task<string[]> GetNetFrameworkPackagesFiles(IProjectIdentifier projectIdentifier)
        {
            var searchCodeRequest = new SearchCodeRequest() { FileName = "packages.config", Path = projectIdentifier.Path };
            searchCodeRequest.Repos.Add($"{_configurationRoot["GithubOrganization"]}/{projectIdentifier.RepositoryName}");
            var searchResult = await _gitHubClient.Search.SearchCode(searchCodeRequest);
            return searchResult.Items.Select(x => x.Url.Substring(x.Url.IndexOf("contents") + 8)).ToArray(); // blablabla/contents/{filepath}
        }

        private async Task<string[]> GetNetCore2PackagesFiles(IProjectIdentifier projectIdentifier)
        {
            var searchCodeRequest = new SearchCodeRequest() { FileName = "*.csproj", Path = projectIdentifier.Path };
            searchCodeRequest.Repos.Add($"{_configurationRoot["GithubOrganization"]}/{projectIdentifier.RepositoryName}");
            var searchResult = await _gitHubClient.Search.SearchCode(searchCodeRequest);
            return searchResult.Items.Select(x => x.Url.Substring(x.Url.IndexOf("contents") + 8)).ToArray(); // blablabla/contents/{filepath}
        }

        private string GetDownloadedFileContent(RepositoryContent downloadedFile)
        {
            // need to do this because https://stackoverflow.com/questions/2111586/parsing-xml-string-to-an-xml-document-fails-if-the-string-begins-with-xml
            var downloadedFileContent = downloadedFile.Content;
            var startIndex = downloadedFileContent.IndexOf('<');
            if (startIndex > 0)
            {
                downloadedFileContent = downloadedFileContent.Remove(0, startIndex);
            }
            return downloadedFileContent;
        }
    }
}
