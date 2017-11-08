namespace NugetVisualizer.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Dto;
    using NugetVisualizer.Core.Exceptions;
    using NugetVisualizer.Core.Repositories;

    using Octokit;

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

        private async Task<(Project, int)> ParseProjectAsync(IProjectIdentifier projectIdentifier, int snapshotVersion)
        {
            var packagesContents = await _packageReader.GetPackagesContentsAsync(projectIdentifier);
            var project = new Project(projectIdentifier.SolutionName);
            var groupedPackagesByVersion = packagesContents.SelectMany(x => _packageParser.ParsePackages(x)).GroupBy(package => new { package.Name, package.Version }); // getting the first item of the group is fancy version of "distinct"
            var packages = groupedPackagesByVersion.Select(group => group.First()).ToList();
            _packageRepository.AddRange(packages);
            _projectRepository.Add(project, packages.Select(p => p.Id), snapshotVersion);

            return (project, packages.Count);
        }

        public async Task<ProjectParsingResult> ParseProjectsAsync(IEnumerable<IProjectIdentifier> projectIdentifiers, int snapshotVersion)
        {
            var projectList = new List<ParsedProject>();
            bool allExistingProjectsParsed = false;
            bool fatalParsingError;
            var parsingErrors = new List<string>();
            string lastSuccessfullParsedProjectName = null;
            foreach (var projectIdentifier in projectIdentifiers)
            {
                Project project;
                (Project, int) projectParseResult = (null, 0);
                fatalParsingError = false;
                try
                {
                    projectParseResult = await ParseProjectAsync(projectIdentifier, snapshotVersion);
                    project = projectParseResult.Item1;
                }
                catch (CannotGetPackagesContentsException e)
                {
                    fatalParsingError = true;
                    project = null;
                    parsingErrors.Add($"Project {projectIdentifier.SolutionName} cannot be parsed : {e.Message}");
                }
                catch (IOException e)
                {
                    project = null;
                    parsingErrors.Add($"Project {projectIdentifier.SolutionName} cannot be parsed : {e.Message}");
                }
                catch (ApiValidationException apiValidationException)
                {
                    project = null;
                    parsingErrors.Add($"Project {projectIdentifier.SolutionName} cannot be parsed : {apiValidationException.Message}");
                }
                
                if (project != null)
                {
                    projectList.Add(new ParsedProject(projectIdentifier.SolutionName, projectIdentifier.RepositoryName, projectParseResult.Item2));
                    lastSuccessfullParsedProjectName = project.Name;
                }
                allExistingProjectsParsed = project != null && !parsingErrors.Any();
                if (!allExistingProjectsParsed && fatalParsingError)
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
                _projectParsingState.DeleteLatestParsedProject();
            }
            return new ProjectParsingResult(projectList, parsingErrors, allExistingProjectsParsed);
        }
    }
}