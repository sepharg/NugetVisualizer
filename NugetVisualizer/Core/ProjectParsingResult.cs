using System.Collections.Generic;

namespace NugetVisualizer.Core
{
    using NugetVisualizer.Core.Domain;

    public class ProjectParsingResult
    {
        public ProjectParsingResult(List<Project> parsedProjects, List<string> parsingErrors, bool allExistingProjectsParsed)
        {
            ParsedProjects = parsedProjects;
            AllExistingProjectsParsed = allExistingProjectsParsed;
            ParsingErrors = parsingErrors;
        }

        public List<Project> ParsedProjects { get; }

        public List<string> ParsingErrors { get; }

        public bool AllExistingProjectsParsed { get; }
    }
}
