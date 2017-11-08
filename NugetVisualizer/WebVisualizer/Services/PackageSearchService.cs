namespace WebVisualizer.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    using WebVisualizer.Models;

    public class PackageSearchService
    {
        private readonly IPackageRepository _packageRepository;

        private readonly IProjectRepository _projectRepository;
        
        public PackageSearchService(IPackageRepository packageRepository, IProjectRepository projectRepository)
        {
            _packageRepository = packageRepository;
            _projectRepository = projectRepository;
        }

        public List<Package> GetPackagesForProject(string projectName, int snapshotVersion)
        {
            return _packageRepository.GetPackagesForProject(projectName, snapshotVersion);
        }

        public async Task<Dictionary<Package, int>> GetPackagesOrderedByVersions(int snapshotVersion)
        {
            return await _packageRepository.GetPackagesOrderedByVersionsCountAsync(snapshotVersion);
        }

        public async Task<List<string>> GetPackageVersions(string packageName, int snapshotVersion)
        {
            return await _packageRepository.GetPackageVersions(packageName, snapshotVersion);
        }

        public async Task<List<ProjectRow>> GetProjectRows(string packageName, List<string> packageVersionsList, int snapshotVersion)
        {
            var projectRows = new List<ProjectRow>();
            var projectsThatContainPackage = await _projectRepository.GetProjectsForPackage(packageName, snapshotVersion);
            foreach (var project in projectsThatContainPackage)
            {
                var projectRow = new ProjectRow() { ProjectName = project.Name };
                int i = 1;
                foreach (var packageVersion in packageVersionsList)
                {
                    if (project.ProjectPackages.Where(x => x.Package.Name == packageName).Any(pp => pp.Package.Version.Equals(packageVersion)))
                    {
                        projectRow.ValuesList.Add(true);
                    }
                    else
                    {
                        projectRow.ValuesList.Add(false);
                    }
                    i++;
                }
                projectRows.Add(projectRow);
            }
            return projectRows;
        }
    }
}
