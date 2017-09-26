namespace NugetVisualizer.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Exceptions;
    using NugetVisualizer.Core.Repositories;

    using Project = Domain.Project;

    public class ProjectParser : IProjectParser
    {
        private IPackageReader _packageReader;
        private IPackageParser _packageParser;
        private IProjectRepository _projectRepository;
        private IPackageRepository _packageRepository;
        private readonly IProjectParsingState _projectParsingState;

        public delegate ProjectParser Factory(ProjectParserType type);

        public ProjectParser(IPackageReader packageReader, IPackageParser packageParser, IProjectRepository projectRepository, IPackageRepository packageRepository, IProjectParsingState projectParsingState)
        {
            _packageReader = packageReader;
            _packageParser = packageParser;
            _projectRepository = projectRepository;
            _packageRepository = packageRepository;
            _projectParsingState = projectParsingState;
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
            catch (CannotGetPackagesContentsException)
            {
                return null;
            }
        }

        public async Task<ProjectParsingResult> ParseProjectsAsync(IEnumerable<IProjectIdentifier> projectIdentifiers)
        {
            var projectList = new List<Project>();
            bool allExistingProjectsParsed = false;
            string lastSuccessfullParsedProjectName = null;
            foreach (var projectIdentifier in projectIdentifiers)
            {
                var project = await ParseProjectAsync(projectIdentifier);
                if (project != null)
                {
                    projectList.Add(project);
                    lastSuccessfullParsedProjectName = project.Name;
                }
                allExistingProjectsParsed = project != null;
                if (!allExistingProjectsParsed)
                {
                    if (!string.IsNullOrWhiteSpace(lastSuccessfullParsedProjectName))
                    {
                        _projectParsingState.SaveLatestParsedProject(lastSuccessfullParsedProjectName);
                    }
                    break;
                }
            }
            if (allExistingProjectsParsed)
            {
                _projectParsingState.SaveLatestParsedProject(string.Empty);
            }
            return new ProjectParsingResult(projectList, allExistingProjectsParsed);
        }
    }
}