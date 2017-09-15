namespace NugetVisualizer.Core.Github
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Microsoft.Extensions.Configuration;

    using NugetVisualizer.Core.Domain;

    using Octokit;
    using Octokit.Internal;

    public class GithubPackageReader : IPackageReader
    {
        private IConfigurationRoot _configurationRoot;

        private GitHubClient _gitHubClient;

        public GithubPackageReader(IConfigurationHelper configurationHelper)
        {
            _configurationRoot = configurationHelper.GetConfiguration();
            InMemoryCredentialStore credentials = new InMemoryCredentialStore(new Credentials(_configurationRoot["GithubToken"]));
            _gitHubClient = new GitHubClient(new ProductHeaderValue(_configurationRoot["GithubOrganization"]), credentials);
        }

        public List<XDocument> GetPackagesContents(IProjectIdentifier projectIdentifier)
        {
            return GetPackagesContentsAsync(projectIdentifier).GetAwaiter().GetResult();
        }

        public async Task<List<XDocument>> GetPackagesContentsAsync(IProjectIdentifier projectIdentifier)
        {
            var ret = new List<XDocument>();
            foreach (var packagesFile in await GetPackagesFiles(projectIdentifier))
            {
                var downloadedFile = Enumerable.Single<RepositoryContent>((await _gitHubClient.Repository.Content.GetAllContents(_configurationRoot["GithubOrganization"], projectIdentifier.Name, packagesFile)));
                
                var downloadedFileContent = GetDownloadedFileContent(downloadedFile);
                XDocument xml = XDocument.Parse(downloadedFileContent);
                ret.Add(xml);
            }
            return ret;
        }

        private async Task<string[]> GetPackagesFiles(IProjectIdentifier projectIdentifier)
        {
            var searchCodeRequest = new SearchCodeRequest() { FileName = "packages.config" };
            searchCodeRequest.Repos.Add($"{_configurationRoot["GithubOrganization"]}/{projectIdentifier.Name}");
            var searchResult = await _gitHubClient.Search.SearchCode(searchCodeRequest);
            return Enumerable.Select<SearchCode, string>(searchResult.Items, x => x.Url.Substring(x.Url.IndexOf("contents") + 8)).ToArray(); // blablabla/contents/{filepath}
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
