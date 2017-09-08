namespace NugetVisualizer.Core.Github
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;

    public class GithubProjectParser : IProjectParser
    {
        private GithubPackageReader _githubPackageReader;

        private PackageParser _packageParser;

        public GithubProjectParser()
        {
            _githubPackageReader = new GithubPackageReader();
            _packageParser = new PackageParser();
        }

        public Project ParseProject(IProjectIdentifier projectIdentifier)
        {
            return ParseProjectAsync(projectIdentifier).GetAwaiter().GetResult();
        }

        public async Task<Project> ParseProjectAsync(IProjectIdentifier projectIdentifier)
        {
            var packagesContents = await _githubPackageReader.GetPackagesContentsAsync(projectIdentifier);
            var project = new Project(projectIdentifier.Name);
            project.Packages.AddRange(packagesContents.SelectMany(x => _packageParser.ParsePackages(x)));

            return project;
        }

        public List<Project> ParseProjects(IEnumerable<IProjectIdentifier> projectIdentifiers)
        {
            return projectIdentifiers.Select(ParseProject).ToList();
        }

        public async Task<List<Project>> ParseProjectsAsync(IEnumerable<IProjectIdentifier> projectIdentifiers)
        {
            var projectList = new List<Project>();
            foreach (var projectIdentifier in projectIdentifiers)
            {
                var project = await ParseProjectAsync(projectIdentifier);
                projectList.Add(project);
            }
            return projectList;
        }
    }
}