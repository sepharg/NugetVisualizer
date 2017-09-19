using System;
using System.Collections.Generic;
using System.Text;

namespace NugetVisualizer.Core
{
    using NugetVisualizer.Core.Domain;

    public class ProjectParsingResult
    {
        public ProjectParsingResult(List<Project> parsedProjects, bool allExistingProjectsParsed)
        {
            ParsedProjects = parsedProjects;
            AllExistingProjectsParsed = allExistingProjectsParsed;
        }

        public List<Project> ParsedProjects { get; }

        public bool AllExistingProjectsParsed { get; }
    }
}
