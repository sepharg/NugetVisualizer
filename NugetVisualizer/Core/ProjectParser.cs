namespace NugetVisualizer.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Exceptions;
    using NugetVisualizer.Core.Repositories;

    using Octokit;

    using Project = NugetVisualizer.Core.Domain.Project;

    public class ProjectParser : IProjectParser
    {
        private IPackageReader _packageReader;
        private IPackageParser _packageParser;
        private IProjectRepository _projectRepository;
        private IPackageRepository _packageRepository;

        public delegate ProjectParser Factory(ProjectParserType type);

        public ProjectParser(IPackageReader packageReader, IPackageParser packageParser, IProjectRepository projectRepository, IPackageRepository packageRepository)
        {
            _packageReader = packageReader;
            _packageParser = packageParser;
            _projectRepository = projectRepository;
            _packageRepository = packageRepository;
        }

        private async Task<Project> ParseProjectAsync(IProjectIdentifier projectIdentifier)
        {
            try
            {
                var packagesContents = await _packageReader.GetPackagesContentsAsync(projectIdentifier);
                var project = new Project(projectIdentifier.Name);
                var groupedPackagesByVersion = packagesContents.SelectMany(x => _packageParser.ParsePackages(x)).GroupBy(package => new { package.Name, package.Version }); // getting the first item of the group is fancy version of "distinct"
                var packages = groupedPackagesByVersion.Select(group => group.First()).ToList();
                _packageRepository.AddRange(packages);
                _projectRepository.Add(project, packages.Select(p => p.Id));

                return project;
            }
            catch (CannotGetPackagesContentsException ex)
            {
                return null;
            }
        }

        public async Task<List<Project>> ParseProjectsAsync(IEnumerable<IProjectIdentifier> projectIdentifiers)
        {
            var projectList = new List<Project>();
            foreach (var projectIdentifier in projectIdentifiers)
            {
                var project = await ParseProjectAsync(projectIdentifier);
                if (project != null)
                {
                    projectList.Add(project);
                }
            }
            return projectList;
        }
    }
}