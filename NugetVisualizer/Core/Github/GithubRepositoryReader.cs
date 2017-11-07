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
            InMemoryCredentialStore credentials = new InMemoryCredentialStore(new Credentials(_configurationRoot["GithubToken"]));
            _gitHubClient = _githubClientFactory.GetClient(_configurationRoot["GithubOrganization"], credentials);
        }

        public async Task<List<IProjectIdentifier>> GetProjectsAsync(string rootPath, string[] filters)
        {
            // Github's search API has a custom limit of 30 requests per minute, so we have to throttle otherwise we get kicked out very likely. https://developer.github.com/v3/search/#rate-limit
            var projects = new List<IProjectIdentifier>();
            var result = await _gitHubClient.Repository.GetAllForOrg(rootPath);

            var counter = new Stopwatch();
            int processed = 0;

            counter.Start();
            foreach (var repository in result.Where(x => filters.All(filter => x.Name.ToLowerInvariant().Contains(filter.ToLowerInvariant()))))
            {
                // get all sln files from the repo and treat them as projects
                var searchRequest = new SearchCodeRequest($"filename:*.sln+repo:{rootPath}/{repository.Name}");

                if ((counter.ElapsedMilliseconds <= 1000 * 60) && processed >= 30)
                {
                    counter.Stop();
                    // wait until a full minute has passed, then start again with 30 available requests to process
                    var extraSlack = 3000;
                    int remainingTimeUntilMinuteHasPassed = (int)(((1000 * 60) + extraSlack) - counter.ElapsedMilliseconds);
                    await Task.Delay(remainingTimeUntilMinuteHasPassed).ConfigureAwait(false);
                    counter.Start();
                    processed = 0;
                }
                var searchResult = await _gitHubClient.Search.SearchCode(searchRequest);
                processed++;

                foreach (var searchResultItem in searchResult.Items)
                {
                    var projectPath = searchResultItem.Path;
                    string solutionName = String.Empty;
                    string solutionPath = String.Empty;
                    
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

                    projects.Add(new ProjectIdentifier(solutionName, repository.Name, solutionPath));
                }
            }
            return projects;
        }

        public List<IProjectIdentifier> GetProjects(string rootPath, string[] filters)
        {
            return GetProjectsAsync(rootPath, filters).GetAwaiter().GetResult();
        }
    }
}
