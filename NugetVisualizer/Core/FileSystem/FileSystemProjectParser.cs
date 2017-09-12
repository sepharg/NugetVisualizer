namespace NugetVisualizer.Core.FileSystem
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    public class FileSystemProjectParser : IProjectParser
    {
        private FileSystemPackageReader _fileSystemPackageReader;

        private PackageParser _packageParser;

        private ProjectRepository _projectRepository;

        private PackageRepository _packageRepository;

        public FileSystemProjectParser()
        {
            _fileSystemPackageReader = new FileSystemPackageReader();
            _packageParser = new PackageParser();
            _projectRepository = new ProjectRepository(new ConfigurationHelper());
            _packageRepository = new PackageRepository();
        }

        public Project ParseProject(IProjectIdentifier projectIdentifier)
        {
            var packagesContents = _fileSystemPackageReader.GetPackagesContents(projectIdentifier);
            var project = new Project(projectIdentifier.Name);
            var packages = packagesContents.SelectMany(x => _packageParser.ParsePackages(x)).GroupBy(package => new { package.Name, package.Version }).Select(group => group.First()).ToList();
            _packageRepository.AddRange(packages);
            _projectRepository.Add(project, packages);

            return project;
        }

        public Task<Project> ParseProjectAsync(IProjectIdentifier projectIdentifier)
        {
            return Task.FromResult(ParseProject(projectIdentifier));
        }

        public List<Project> ParseProjects(IEnumerable<IProjectIdentifier> projectIdentifiers)
        {
            return projectIdentifiers.Select(ParseProject).ToList();
        }

        public Task<List<Project>> ParseProjectsAsync(IEnumerable<IProjectIdentifier> projectIdentifiers)
        {
            return Task.FromResult(ParseProjects(projectIdentifiers));
        }
    }
}
