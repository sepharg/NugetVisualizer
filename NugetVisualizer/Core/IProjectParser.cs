namespace NugetVisualizer.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;

    public interface IProjectParser
    {
        Task<List<Project>> ParseProjectsAsync(IEnumerable<IProjectIdentifier> projectIdentifiers);
    }
}
