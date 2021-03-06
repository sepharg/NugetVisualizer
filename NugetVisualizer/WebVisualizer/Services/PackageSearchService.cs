﻿namespace WebVisualizer.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Nuget;
    using NugetVisualizer.Core.Repositories;

    using WebVisualizer.Models;

    public class PackageSearchService
    {
        private readonly IPackageRepository _packageRepository;

        private readonly IProjectRepository _projectRepository;

        private readonly NugetVersionQuery _nugetVersionQuery;

        public PackageSearchService(IPackageRepository packageRepository, IProjectRepository projectRepository, NugetVersionQuery nugetVersionQuery)
        {
            _packageRepository = packageRepository;
            _projectRepository = projectRepository;
            _nugetVersionQuery = nugetVersionQuery;
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

        public async Task<string> GetPackageLatestVersion(string packageName)
        {
            return await _nugetVersionQuery.GetLatestVersion(packageName);
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
                    if (project.ProjectPackages.Where(x => x.Package.Name == packageName && x.SnapshotVersion.Equals(snapshotVersion)).Any(pp => pp.Package.Version.Equals(packageVersion)))
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
