using System;
using System.Collections.Generic;
using System.Text;

namespace NugetVisualizer.Core.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    using NugetVisualizer.Core.Domain;

    public class NugetVisualizerContext : DbContext
    {
        private readonly IConfigurationHelper _configurationHelper;

        private IConfigurationRoot _configurationRoot;

        public const string _databaseName = "nugetVisualizerdb";

        public NugetVisualizerContext(IConfigurationHelper configurationHelper)
        {
            _configurationRoot = configurationHelper.GetConfiguration();
        }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Package> Packages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_configurationRoot["Dbpath"]}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectPackage>().HasKey(x => new { x.ProjectName, x.PackageId });
            modelBuilder.Entity<ProjectPackage>().HasOne(x => x.Project)
                                                 .WithMany(x => x.ProjectPackages)
                                                 .HasForeignKey(x => x.ProjectName);
            modelBuilder.Entity<ProjectPackage>().HasOne(x => x.Package)
                                                .WithMany(x => x.ProjectPackages)
                                                .HasForeignKey(x => x.PackageId);
            modelBuilder.Entity<Project>().HasKey(x => x.Name);
            //modelBuilder.Entity<Package>().HasKey(x => new { x.Name, x.Version });
            modelBuilder.Entity<Package>().HasKey(x => x.Id);
            modelBuilder.Entity<Package>().HasIndex(x => new { x.Name, x.Version }).IsUnique();
        }
    }
}
