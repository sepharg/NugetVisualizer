﻿using System.Collections.Generic;
using NugetVisualizer.Core.Domain;

namespace NugetVisualizer.Core.Repositories
{
    using System;

    public interface IPackageRepository
    {
        void Add(Package package);

        void AddRange(IEnumerable<Package> packages);

        List<Package> GetPackages(Func<Package, string> orderBy = null);

        Dictionary<Package, int> GetPackagesOrderedByVersionsCount();

        Dictionary<Package, int> GetMostUsedPackages(int? maxNumberToRetrieve = null);

        List<string> GetPackageVersions(string packageName);
    }
}