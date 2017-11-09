namespace NugetVisualizer.Core.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using NugetVisualizer.Core.Domain;

    public class ProjectRepository : IDisposable, IProjectRepository
    {
        private readonly INugetVisualizerContext _dbContext;

        public ProjectRepository(INugetVisualizerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Project> LoadProjects(int snapshotVersion)
        {
            return _dbContext.Projects.Where(p => p.ProjectPackages.Any(pp => pp.SnapshotVersion == snapshotVersion))
                                      .Include(x => x.ProjectPackages)
                                      .ThenInclude(y => y.Package)
                                      .ToList();
        }

        public async Task<List<Project>> GetProjectsForPackage(string packageName, int snapshotVersion)
        {
            return await _dbContext.Projects.Where(p => p.ProjectPackages.Any(pp => pp.Package.Name.Equals(packageName) && pp.SnapshotVersion == snapshotVersion))
                                            .Include(x => x.ProjectPackages)
                                            .ThenInclude(x => x.Package)
                                            .ToListAsync();
        }

        public void Add(Project project, IEnumerable<int> packageIds, int snapshotVersion)
        {
            var existingProject = _dbContext.Projects.Include(x => x.ProjectPackages)
                                                     .SingleOrDefault(x => x.Name == project.Name);
            if (existingProject == null)
            {
                foreach (var packageId in packageIds)
                {
                    project.ProjectPackages.Add(new ProjectPackage() { ProjectName = project.Name, PackageId = packageId, SnapshotVersion = snapshotVersion});
                }
                _dbContext.Projects.Add(project);
                _dbContext.SaveChanges();
            }
            else
            {
                foreach (var packageId in packageIds.Except(existingProject.ProjectPackages.Where(pp => pp.SnapshotVersion == snapshotVersion).Select(x => x.PackageId)))
                {
                    existingProject.ProjectPackages.Add(new ProjectPackage() { ProjectName = existingProject.Name, PackageId = packageId, SnapshotVersion = snapshotVersion });
                }
                _dbContext.SaveChanges();
            }
        }
        
        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
