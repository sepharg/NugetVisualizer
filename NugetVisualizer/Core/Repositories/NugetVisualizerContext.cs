namespace NugetVisualizer.Core.Repositories
{
    using System.Data.Common;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    using NugetVisualizer.Core.Domain;

    public class NugetVisualizerContext : DbContext, INugetVisualizerContext
    {
        private readonly IConfigurationHelper _configurationHelper;

        public NugetVisualizerContext(IConfigurationHelper configurationHelper, DbContextOptions<NugetVisualizerContext> options) : base(options)
        {
            _configurationHelper = configurationHelper;
        }

        public virtual DbSet<Snapshot> Snapshots { get; set; }

        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<Package> Packages { get; set; }

        public virtual DbSet<ProjectPackage> ProjectPackages { get; set; }

        public DbConnection GetDbConnection()
        {
            return Database.GetDbConnection();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configurationHelper.UseSqlLite)
            {
                optionsBuilder.UseSqlite($"Data Source={_configurationHelper.Dbpath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Snapshot>().HasKey(x => x.Version);
            modelBuilder.Entity<Snapshot>().Property(x => x.Version).ValueGeneratedOnAdd();
            modelBuilder.Entity<ProjectPackage>().HasKey(x => new { x.ProjectName, x.PackageId, x.SnapshotVersion });
            modelBuilder.Entity<ProjectPackage>().HasOne(x => x.Project)
                                                 .WithMany(x => x.ProjectPackages)
                                                 .HasForeignKey(x => x.ProjectName);
            modelBuilder.Entity<ProjectPackage>().HasOne(x => x.Package)
                                                .WithMany(x => x.ProjectPackages)
                                                .HasForeignKey(x => x.PackageId);
            modelBuilder.Entity<Project>().HasKey(x => x.Name);
            modelBuilder.Entity<Package>().HasKey(x => x.Id);
            modelBuilder.Entity<Package>().HasIndex(x => new { x.Name, x.Version }).IsUnique();
        }
    }
}
