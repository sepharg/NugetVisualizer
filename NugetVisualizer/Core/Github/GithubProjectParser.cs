namespace NugetVisualizer.Core.Github
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    public class GithubProjectParser : IProjectParser
    {
        private GithubPackageReader _githubPackageReader;

        private PackageParser _packageParser;

        private ProjectRepository _projectRepository;

        private PackageRepository _packageRepository;

        public GithubProjectParser()
        {
            _githubPackageReader = new GithubPackageReader(new ConfigurationHelper());
            _packageParser = new PackageParser();
            _projectRepository = new ProjectRepository(new ConfigurationHelper());
            _packageRepository = new PackageRepository();
        }

        public Project ParseProject(IProjectIdentifier projectIdentifier)
        {
            return ParseProjectAsync(projectIdentifier).GetAwaiter().GetResult();
        }

        public async Task<Project> ParseProjectAsync(IProjectIdentifier projectIdentifier)
        {
            var packagesContents = await _githubPackageReader.GetPackagesContentsAsync(projectIdentifier);
            var project = new Project(projectIdentifier.Name);
            var packages = packagesContents.SelectMany(x => _packageParser.ParsePackages(x)).GroupBy(package => new { package.Name, package.Version }).Select(group => group.First()).ToList();
            _packageRepository.AddRange(packages);
            _projectRepository.Add(project, packages.Select(p => p.Id));

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