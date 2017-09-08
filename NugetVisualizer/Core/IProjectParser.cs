namespace NugetVisualizer.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;

    public interface IProjectParser
    {
        Project ParseProject(IProjectIdentifier projectIdentifier);
        Task<Project> ParseProjectAsync(IProjectIdentifier projectIdentifier);

        List<Project> ParseProjects(IEnumerable<IProjectIdentifier> projectIdentifiers);
        Task<List<Project>> ParseProjectsAsync(IEnumerable<IProjectIdentifier> projectIdentifiers);
    }
}
