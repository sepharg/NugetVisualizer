namespace NugetVisualizer.Core.FileSystem
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;

    public class FileSystemProjectParser : IProjectParser
    {
        private FileSystemPackageReader _fileSystemPackageReader;

        private PackageParser _packageParser;

        public FileSystemProjectParser()
        {
            _fileSystemPackageReader = new FileSystemPackageReader();
            _packageParser = new PackageParser();
        }

        public Project ParseProject(IProjectIdentifier projectIdentifier)
        {
            var packagesContents = _fileSystemPackageReader.GetPackagesContents(projectIdentifier);
            var project = new Project(projectIdentifier.Name);
            project.Packages.AddRange(Enumerable.SelectMany(packagesContents, x => _packageParser.ParsePackages(x)));

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
