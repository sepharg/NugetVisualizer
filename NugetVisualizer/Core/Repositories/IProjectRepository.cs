using System.Collections.Generic;
using NugetVisualizer.Core.Domain;

namespace NugetVisualizer.Core.Repositories
{
    using System.Threading.Tasks;

    public interface IProjectRepository
    {
        void Add(Project project, IEnumerable<int> packageIds, int snapshotVersion);
        List<Project> LoadProjects();

        Task<List<Project>> GetProjectsForPackage(string packageName, int snapshotVersion);
    }
}