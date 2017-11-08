namespace NugetVisualizer.Core.Github
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;

    using NugetVisualizer.Core.Domain;

    using Octokit;
    using Octokit.Internal;

    public class GithubRepositoryReader : IRepositoryReader
    {
        private readonly IGithubClientFactory _githubClientFactory;

        private IConfigurationRoot _configurationRoot;

        private IGitHubClient _gitHubClient;

        public GithubRepositoryReader(IConfigurationHelper configurationHelper, IGithubClientFactory githubClientFactory)
        {
            _githubClientFactory = githubClientFactory;
            _configurationRoot = configurationHelper.GetConfiguration();
            var credentials = new InMemoryCredentialStore(new Credentials(_configurationRoot["GithubToken"]));
            _gitHubClient = _githubClientFactory.GetClient(_configurationRoot["GithubOrganization"], credentials);
        }

        public async Task<List<IProjectIdentifier>> GetProjectsAsync(string rootPath, string[] filters)
        {
            // Github's search API has a custom limit of 30 requests per minute, so we have to throttle otherwise we get kicked out very likely. https://developer.github.com/v3/search/#rate-limit
            var projects = new List<IProjectIdentifier>();
            
            var searchRequest = new SearchCodeRequest($"org:{rootPath} filename:*.sln");
            var keepSearching = true;
            var page = 0;
            var parsedSoFar = 0;

            do
            {
                searchRequest.Page = ++page;
                var searchResult = await _gitHubClient.Search.SearchCode(searchRequest);

                foreach (var searchResultItem in searchResult.Items.Where(x => filters.All(filter => x.Repository.Name.ToLowerInvariant().Contains(filter.ToLowerInvariant()))))
                {
                    var projectPath = searchResultItem.Path;
                    string solutionName;
                    var solutionPath = string.Empty;

                    if (projectPath.LastIndexOf('/') != -1)
                    {
                        solutionName = projectPath.Substring(projectPath.LastIndexOf('/') + 1)
                            .Substring(0, projectPath.Substring(projectPath.LastIndexOf('/') + 1).Length - 4);
                        solutionPath = projectPath.Substring(0, projectPath.LastIndexOf("/"));
                    }
                    else
                    {
                        solutionName = projectPath.Substring(0, projectPath.Length - 4);
                    }

                    projects.Add(new ProjectIdentifier(solutionName, searchResultItem.Repository.Name, solutionPath));
                }
                parsedSoFar += searchResult.Items.Count;
                keepSearching = searchResult.TotalCount > parsedSoFar;
            }
            while (keepSearching);
            
            return projects.OrderBy(proj => proj.RepositoryName).ToList();
        }

        public List<IProjectIdentifier> GetProjects(string rootPath, string[] filters)
        {
            return GetProjectsAsync(rootPath, filters).GetAwaiter().GetResult();
        }
    }
}
