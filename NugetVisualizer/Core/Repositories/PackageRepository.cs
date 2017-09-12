using System;
using System.Collections.Generic;
using System.Text;

namespace NugetVisualizer.Core.Repositories
{
    using System.Linq;

    using NugetVisualizer.Core.Domain;

    public class PackageRepository
    {
        public void Add(Package package)
        {
            using (var db = new NugetVisualizerContext(new ConfigurationHelper()))
            {
                if (db.Packages.SingleOrDefault(x => x.Name == package.Name && x.Version == package.Version) == null)
                {
                    db.Packages.Add(package);
                    db.SaveChanges();
                }
            }
        }

        public void AddRange(IEnumerable<Package> packages)
        {
            using (var db = new NugetVisualizerContext(new ConfigurationHelper()))
            {
                foreach (var package in packages.GroupBy(x => new { x.Name, x.Version }).Select(g => g.First()))
                {
                    var existingPackage = db.Packages.SingleOrDefault(x => x.Name == package.Name && x.Version == package.Version);
                    if (existingPackage == null)
                    {
                        db.Packages.Add(package);
                    }
                    else
                    {
                        package.Id = existingPackage.Id;
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
