﻿using System.Collections.Generic;
using NugetVisualizer.Core.Domain;

namespace NugetVisualizer.Core.Repositories
{
    using System;
    using System.Threading.Tasks;

    public interface IPackageRepository
    {
        void Add(Package package);

        void AddRange(IEnumerable<Package> packages);

        List<Package> GetPackages(Func<Package, string> orderBy = null);

        List<Package> GetPackagesForProject(string projectName, int snapshotVersion);

        Task<Dictionary<Package, int>> GetPackagesOrderedByVersionsCountAsync(int snapshotVersion);

        Task<Dictionary<Package, int>> GetPackageUsesAsync(int snapshotVersion);

        Task<List<string>> GetPackageVersions(string packageName, int snapshotVersion);
    }
}