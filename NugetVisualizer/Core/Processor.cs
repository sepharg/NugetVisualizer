namespace NugetVisualizer.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    using Octokit;

    using Project = NugetVisualizer.Core.Domain.Project;

    public class Processor : IProcessor
    {
        private readonly IProjectParser _projectParser;

        private readonly IRepositoryReader _repositoryReader;

        private readonly IProjectParsingState _projectParsingState;

        private readonly ISnapshotRepository _snapshotRepository;

        public Processor(IProjectParser projectParser, IRepositoryReader repositoryReader, IProjectParsingState projectParsingState, ISnapshotRepository snapshotRepository)
        {
            _projectParser = projectParser;
            _repositoryReader = repositoryReader;
            _projectParsingState = projectParsingState;
            _snapshotRepository = snapshotRepository;
        }

        public async Task<ProjectParsingResult> Process(string rootPath, string[] filters, int snapshotVersion)
        {
            var latestParsedProject = _projectParsingState.GetLatestParsedProject();
            List<IProjectIdentifier> projectIdentifiers;
            try
            {
                projectIdentifiers = await _repositoryReader.GetProjectsAsync(rootPath, filters);
            }
            catch (RateLimitExceededException limitExceededException)
            {
                return new ProjectParsingResult(new List<Project>(), new List<string> { $"Couldn't get the list of projects : {limitExceededException.Message}" }, false);
            }
            
            if (string.IsNullOrEmpty(latestParsedProject))
            {
                return await _projectParser.ParseProjectsAsync(projectIdentifiers, snapshotVersion);
            }

            var alreadyProcessed = projectIdentifiers.FindIndex(pi => pi.Name.Equals(latestParsedProject)) + 1;
            var remainingProjectsToParse = projectIdentifiers.Skip(alreadyProcessed);
            return await _projectParser.ParseProjectsAsync(remainingProjectsToParse, snapshotVersion);
        }

        public Task<ProjectParsingResult> Process(string rootPath, string[] filters, string snapshotName)
        {
            var snapshot = new Snapshot() { Name = snapshotName };
            _snapshotRepository.Add(snapshot);
            return Process(rootPath, filters, snapshot.Version);
        }
    }
}
