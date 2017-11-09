namespace WebVisualizer.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    public class ProjectSearchService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectSearchService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<List<Project>> GetProjects(int snapshotVersion)
        {
            return await Task.FromResult(_projectRepository.LoadProjects(snapshotVersion)); 
        }
    }
}
