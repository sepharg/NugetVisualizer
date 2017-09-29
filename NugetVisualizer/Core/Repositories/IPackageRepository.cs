using System.Collections.Generic;
using NugetVisualizer.Core.Domain;

namespace NugetVisualizer.Core.Repositories
{
    public interface IPackageRepository
    {
        void Add(Package package);
        void AddRange(IEnumerable<Package> packages);
        List<Package> LoadPackages();

        List<string> GetPackageVersions(string packageName);
    }
}