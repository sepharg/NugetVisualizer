namespace NugetVisualizer.Core.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using NugetVisualizer.Core.Domain;

    public class ProjectRepository : IDisposable, IProjectRepository
    {
        private readonly INugetVisualizerContext _dbContext;

        public ProjectRepository(INugetVisualizerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Project> LoadProjects()
        {
            return _dbContext.Projects.Include(x => x.ProjectPackages).ThenInclude(y => y.Package).ToList();
        }

        public List<Project> GetProjectsForPackage(string packageName)
        {
            return _dbContext.Projects.Where(p => p.ProjectPackages.Any(pp => pp.Package.Name.Equals(packageName)))
                                      .Include(x => x.ProjectPackages)
                                      .ThenInclude(x => x.Package)
                                      .ToList();
        }

        public void Add(Project project, IEnumerable<int> packageIds, int snapshotVersion)
        {
            var existingProject = _dbContext.Projects.SingleOrDefault(x => x.Name == project.Name);
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
                foreach (var packageId in packageIds.Except(project.ProjectPackages.Where(pp => pp.SnapshotVersion == snapshotVersion).Select(x => x.PackageId)))
                {
                    project.ProjectPackages.Add(new ProjectPackage() { ProjectName = project.Name, PackageId = packageId, SnapshotVersion = snapshotVersion });
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
