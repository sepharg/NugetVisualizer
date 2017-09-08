using System;
using System.Collections.Generic;
using System.Text;

namespace NugetVisualizer.Core.Github
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;

    using NugetVisualizer.Core.Domain;

    using Octokit;
    using Octokit.Internal;

    public class GithubRepositoryReader : IRepositoryReader
    {
        private IConfigurationRoot _configurationRoot;

        private GitHubClient _gitHubClient;

        public GithubRepositoryReader(ConfigurationHelper configurationHelper)
        {
            _configurationRoot = configurationHelper.GetConfiguration();
            InMemoryCredentialStore credentials = new InMemoryCredentialStore(new Credentials(_configurationRoot["GithubToken"]));
            _gitHubClient = new GitHubClient(new ProductHeaderValue(_configurationRoot["GithubOrganization"]), credentials);
        }

        public async Task<List<IProjectIdentifier>> GetProjectsAsync(string rootPath, string[] filters)
        {
            var projects = new List<IProjectIdentifier>();
            var result = await _gitHubClient.Repository.GetAllForOrg(rootPath);
            foreach (var repository in result.Where(x => filters.All(filter => x.Name.ToLowerInvariant().Contains(filter.ToLowerInvariant()))))
            {
                projects.Add(new ProjectIdentifier(repository.Name, ""));
            }
            return projects;
        }

        List<IProjectIdentifier> IRepositoryReader.GetProjects(string rootPath, string[] filters)
        {
            return GetProjectsAsync(rootPath, filters).GetAwaiter().GetResult();
        }
    }
}
