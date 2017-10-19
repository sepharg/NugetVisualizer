namespace NugetVisualizer.Core.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using NugetVisualizer.Core.Domain;

    public class PackageRepository : IDisposable, IPackageRepository
    {
        private readonly INugetVisualizerContext _context;

        private static readonly Func<Package, string> defaultOrderByFunc = (p) => p.Name;

        public PackageRepository(INugetVisualizerContext context)
        {
            _context = context;
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

        public List<Package> GetPackages(Func<Package, string> orderBy = null)
        {
            return _context.Packages.OrderBy(orderBy ?? defaultOrderByFunc).ToList();
        }

        public async Task<Dictionary<Package, int>> GetPackagesOrderedByVersionsCountAsync(int snapshotVersion)
        {
            var conn = _context.GetDbConnection();
            var result = new Dictionary<Package, int>();
            try
            {
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    var query = @"SELECT p.Id, p.Name, p.TargetFramework, p.Version, Count(p.Name) as Count FROM Packages p
                                     WHERE p.Id IN (SELECT PackageId FROM ProjectPackages WHERE SnapshotVersion = " + snapshotVersion + @")
                                     GROUP BY p.Name
                                     ORDER BY Count DESC";
                    command.CommandText = query;
                    var reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var package = new Package(
                                                reader.GetString(1),
                                                reader.GetString(3),
                                                string.IsNullOrEmpty(reader.GetValue(2) as string) ? string.Empty : reader.GetString(2)) { Id = reader.GetInt32(0) };
                            result.Add(package, reader.GetInt32(4));
                        }
                    }
                    reader.Dispose();
                }
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public async Task<Dictionary<Package, int>> GetPackageUsesAsync(int snapshotVersion)
        {
            var result = new Dictionary<Package, int>();
            var conn = _context.GetDbConnection();
            try
            {
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    var query = @"SELECT p.Id, p.Name, p.TargetFramework, p.Version, group_concat(p.Id) as PackageIds FROM Packages p
                                     WHERE p.Id IN (SELECT PackageId FROM ProjectPackages WHERE SnapshotVersion = " + snapshotVersion + @")
                                     GROUP BY p.Name";
                    command.CommandText = query;
                    var reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var package = new Package(
                                              reader.GetString(1),
                                              reader.GetString(3),
                                              string.IsNullOrEmpty(reader.GetValue(2) as string) ? string.Empty : reader.GetString(2))
                                              { Id = reader.GetInt32(0) };

                            var idsForPackage = reader.GetString(4).Split(',').Select(int.Parse);
                            var usagesCountForPackage = _context.ProjectPackages.Count(y => idsForPackage.Contains(y.PackageId) && y.SnapshotVersion == snapshotVersion);
                            result.Add(package, usagesCountForPackage);
                        }
                    }
                    reader.Dispose();
                }
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public List<string> GetPackageVersions(string packageName)
        {
            return _context.Packages.GroupBy(x => x.Name)
                                    .Single(x => x.Key.Equals(packageName))
                                    .OrderBy(x => x.Version)
                                    .Select(x => x.Version)
                                    .ToList();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
