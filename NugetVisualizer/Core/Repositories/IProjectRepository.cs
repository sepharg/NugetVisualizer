using System.Collections.Generic;
using NugetVisualizer.Core.Domain;

namespace NugetVisualizer.Core.Repositories
{
    public interface IProjectRepository
    {
        void Add(Project project, IEnumerable<int> packageIds, int snapshotVersion);
        List<Project> LoadProjects();

        List<Project> GetProjectsForPackage(string packageName);
    }
}