namespace NugetVisualizer.Core
{
    using System.Collections.Generic;
    using System.Linq;

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
            project.Packages.AddRange(packagesContents.SelectMany(x => _packageParser.ParsePackages(x)));

            return project;
        }

        public IEnumerable<Project> ParseProjects(IEnumerable<IProjectIdentifier> projectIdentifiers)
        {
            return projectIdentifiers.Select(ParseProject);
        }
    }
}
