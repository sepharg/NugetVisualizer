namespace NugetVisualizer.Core
{
    using System.Collections.Generic;

    using NugetVisualizer.Core.Domain;

    public interface IProjectParser
    {
        Project ParseProject(IProjectIdentifier projectIdentifier);

        IEnumerable<Project> ParseProjects(IEnumerable<IProjectIdentifier> projectIdentifiers);
    }
}
