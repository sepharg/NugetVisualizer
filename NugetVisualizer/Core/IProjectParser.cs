namespace NugetVisualizer.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Dto;

    public interface IProjectParser
    {
        Task<ProjectParsingResult> ParseProjectsAsync(IEnumerable<IProjectIdentifier> projectIdentifiers, int snapshotVersion);
    }
}
