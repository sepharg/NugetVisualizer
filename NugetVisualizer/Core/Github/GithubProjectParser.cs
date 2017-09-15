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

        private IPackageParser _packageParser;

        private ProjectRepository _projectRepository;

        private PackageRepository _packageRepository;

        public GithubProjectParser(GithubPackageReader githubPackageReader, IPackageParser packageParser, ProjectRepository projectRepository, PackageRepository packageRepository)
        {
            _githubPackageReader = githubPackageReader;
            _packageParser = packageParser;
            _projectRepository = projectRepository;
            _packageRepository = packageRepository;
        }

        private async Task<Project> ParseProjectAsync(IProjectIdentifier projectIdentifier)
        {
            var packagesContents = await _githubPackageReader.GetPackagesContentsAsync(projectIdentifier);
            var project = new Project(projectIdentifier.Name);
            var packages = packagesContents.SelectMany(x => _packageParser.ParsePackages(x)).GroupBy(package => new { package.Name, package.Version }).Select(group => group.First()).ToList();
            _packageRepository.AddRange(packages);
            _projectRepository.Add(project, packages.Select(p => p.Id));

            return project;
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