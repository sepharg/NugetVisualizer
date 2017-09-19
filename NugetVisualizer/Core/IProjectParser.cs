namespace NugetVisualizer.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;

    public interface IProjectParser
    {
        Task<ProjectParsingResult> ParseProjectsAsync(IEnumerable<IProjectIdentifier> projectIdentifiers);
    }
}
