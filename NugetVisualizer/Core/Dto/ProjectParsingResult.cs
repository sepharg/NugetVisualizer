namespace NugetVisualizer.Core.Dto
{
    using System.Collections.Generic;

    using NugetVisualizer.Core.Domain;

    public class ProjectParsingResult
    {
        public ProjectParsingResult(List<ParsedProject> parsedProjects, List<string> parsingErrors, bool allExistingProjectsParsed)
        {
            ParsedProjects = parsedProjects;
            AllExistingProjectsParsed = allExistingProjectsParsed;
            ParsingErrors = parsingErrors;
        }

        public List<ParsedProject> ParsedProjects { get; }

        public List<string> ParsingErrors { get; }

        public bool AllExistingProjectsParsed { get; }
    }
}
