namespace NugetVisualizer.Core.Github
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    using Microsoft.Extensions.Configuration;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Exceptions;
    using NugetVisualizer.Core.PackageParser;

    using Octokit;
    using Octokit.Internal;

    public class GithubPackageReader : IPackageReader
    {
        private IConfigurationRoot _configurationRoot;

        private GitHubClient _gitHubClient;

        private delegate Task<string[]> GetFiles(IProjectIdentifier projectIdentifier);

        public GithubPackageReader(IConfigurationHelper configurationHelper)
        {
            _configurationRoot = configurationHelper.GetConfiguration();
            var githubToken = _configurationRoot["GithubToken"];
            InMemoryCredentialStore credentials = new InMemoryCredentialStore(new Credentials(githubToken));
            _gitHubClient = new GitHubClient(new ProductHeaderValue(_configurationRoot["GithubOrganization"]), credentials);
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
            var searchApiCalls = 0;
            var counter = new Stopwatch();
            counter.Start();
            foreach (var packagesFile in await getFilesDelegate(projectIdentifier))
            {
                var downloadedFile = (await _gitHubClient.Repository.Content.GetAllContents(
                                          _configurationRoot["GithubOrganization"],
                                          projectIdentifier.RepositoryName,
                                          packagesFile)).Single();

                var downloadedFileContent = GetDownloadedFileContent(downloadedFile);
                try
                {
                    XDocument xml = XDocument.Parse(downloadedFileContent);
                    // ToDo: move into its own class / factory / whatever
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
                catch (XmlException e)
                {
                    // ToDo : Log this in a better way
                    Trace.WriteLine($"Error {e.Message} while parsing {packagesFile} for {projectIdentifier.SolutionName}");
                }

                searchApiCalls++;

                // Github's search API has a custom limit of 30 requests per minute, so we have to throttle otherwise we get kicked out. https://developer.github.com/v3/search/#rate-limit 
                if (searchApiCalls == 29 && counter.ElapsedMilliseconds < 60000)
                {
                    int remainingTimeUntilMinuteHasPassed = (int)((1000 * 60) - counter.ElapsedMilliseconds);
                    await Task.Delay(remainingTimeUntilMinuteHasPassed).ConfigureAwait(false);
                    searchApiCalls = 0;
                    counter.Restart();
                }
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

        private class PackageFilesPaths
        {
            public string[] PackageFilePathStrings { get; set; }

            public PackageType PackageType { get; set; }
        }
    }
}
