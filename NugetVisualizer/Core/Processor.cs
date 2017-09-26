namespace NugetVisualizer.Core
{
    using System.Linq;
    using System.Threading.Tasks;

    public class Processor
    {
        private readonly IProjectParser _projectParser;

        private readonly IRepositoryReader _repositoryReader;

        private readonly IProjectParsingState _projectParsingState;

        public Processor(IProjectParser projectParser, IRepositoryReader repositoryReader, IProjectParsingState projectParsingState)
        {
            _projectParser = projectParser;
            _repositoryReader = repositoryReader;
            _projectParsingState = projectParsingState;
        }

        public async Task<ProjectParsingResult> Process(string rootPath, string[] filters)
        {
            var latestParsedProject = _projectParsingState.GetLatestParsedProject();
            var projectIdentifiers = await _repositoryReader.GetProjectsAsync(rootPath, filters);
            if (string.IsNullOrEmpty(latestParsedProject))
            {
                return await _projectParser.ParseProjectsAsync(projectIdentifiers);
            }

            var alreadyProcessed = projectIdentifiers.FindIndex(pi => pi.Name.Equals(latestParsedProject)) + 1;
            var remainingProjectsToParse = projectIdentifiers.Skip(alreadyProcessed);
            return await _projectParser.ParseProjectsAsync(remainingProjectsToParse);
        }
    }
}
