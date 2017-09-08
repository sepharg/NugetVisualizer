namespace NugetVisualizer.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;

    public interface IRepositoryReader
    {
        List<IProjectIdentifier> GetProjects(string rootIdentifier, string[] filters);
        Task<List<IProjectIdentifier>> GetProjectsAsync(string rootIdentifier, string[] filters);
    }
}
