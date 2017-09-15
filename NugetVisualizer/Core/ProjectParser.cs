namespace NugetVisualizer.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    public class ProjectParser : IProjectParser
    {
        private IPackageReader _packageReader;
        private IPackageParser _packageParser;
        private ProjectRepository _projectRepository;
        private PackageRepository _packageRepository;

        public delegate ProjectParser Factory(ProjectParserType type);

        public ProjectParser(IPackageReader packageReader, IPackageParser packageParser, ProjectRepository projectRepository, PackageRepository packageRepository)
        {
            _packageReader = packageReader;
            _packageParser = packageParser;
            _projectRepository = projectRepository;
            _packageRepository = packageRepository;
        }

        private async Task<Project> ParseProjectAsync(IProjectIdentifier projectIdentifier)
        {
            var packagesContents = await _packageReader.GetPackagesContentsAsync(projectIdentifier);
            var project = new Project(projectIdentifier.Name);
            var packages = Enumerable.SelectMany<XDocument, Package>(packagesContents, x => _packageParser.ParsePackages(x)).GroupBy(package => new { package.Name, package.Version }).Select(group => group.First()).ToList();
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