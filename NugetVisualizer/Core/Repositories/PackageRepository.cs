namespace NugetVisualizer.Core.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using NugetVisualizer.Core.Domain;

    public class PackageRepository : IDisposable, IPackageRepository
    {
        private readonly NugetVisualizerContext _context;

        public PackageRepository(DbContext context)
        {
            _context = context as NugetVisualizerContext;
        }

        public void Add(Package package)
        {
            if (_context.Packages.SingleOrDefault(x => x.Name == package.Name && x.Version == package.Version) == null)
            {
                _context.Packages.Add(package);
                _context.SaveChanges();
            }
        }

        public void AddRange(IEnumerable<Package> packages)
        {

            foreach (var package in packages.GroupBy(x => new { x.Name, x.Version }).Select(g => g.First()))
            {
                var existingPackage = _context.Packages.SingleOrDefault(x => x.Name == package.Name && x.Version == package.Version);
                if (existingPackage == null)
                {
                    _context.Packages.Add(package);
                }
                else
                {
                    package.Id = existingPackage.Id;
                }
            }
            _context.SaveChanges();
        }

        public List<Package> LoadPackages()
        {
            return _context.Packages.ToList();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
