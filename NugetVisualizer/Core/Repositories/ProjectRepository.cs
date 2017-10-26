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

        public List<Project> LoadProjects()
        {
            return _dbContext.Projects.Include(x => x.ProjectPackages).ThenInclude(y => y.Package).ToList();
        }

        public async Task<List<Project>> GetProjectsForPackage(string packageName, int snapshotVersion)
        {
            /*var projectsQuery = @"SELECT p.Id, p.Name, p.TargetFramework, p.Version
                                     FROM Packages p
                                     WHERE p.Id IN (SELECT PackageId FROM ProjectPackages WHERE SnapshotVersion = " + snapshotVersion + @")";

            var projectPackagesQuery = @"SELECT ProjectName, PackageId
                                            FROM ProjectPackages
                                            WHERE SnapShotVersion = " + snapshotVersion;

            SqlHelper.ProcessReader<List<Package>> packageProcessReader = (reader, res) =>
                {
                    var package = new Package(
                                      reader.GetString(1),
                                      reader.GetString(3),
                                      string.IsNullOrEmpty(reader.GetValue(2) as string) ? string.Empty : reader.GetString(2))
                                      { Id = reader.GetInt32(0) };
                    res.Add(package);
                };

            SqlHelper.ProcessReader<List<ProjectPackage>> projectPackageProcessReader = (reader, res) =>
                {
                    var projectPackage = new ProjectPackage() { PackageId = reader.GetInt32(1), ProjectName = reader.GetString(0) };
                    res.Add(projectPackage);
                };

            var projects = await _dbContext.GetFromSql(projectsQuery, packageProcessReader);
            var projectPackages = await _dbContext.GetFromSql(projectPackagesQuery, projectPackageProcessReader);*/

            return _dbContext.Projects.Where(p => p.ProjectPackages.Any(pp => pp.Package.Name.Equals(packageName) && pp.SnapshotVersion == snapshotVersion))
                .Include(x => x.ProjectPackages)
                .ThenInclude(x => x.Package)
                .ToList();
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
