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
        private readonly ConfigurationHelper _configurationHelper;

        private IConfigurationRoot _configurationRoot;

        public const string _databaseName = "nugetVisualizerdb";

        public NugetVisualizerContext(ConfigurationHelper configurationHelper)
        {
            _configurationRoot = configurationHelper.GetConfiguration();
        }

        public DbSet<Project> Projects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_configurationRoot["Dbpath"]}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasKey(x => x.Name);
            //modelBuilder.Entity<Package>().HasKey(x => new { x.Name, x.Version });
            modelBuilder.Entity<Package>().HasKey(x => x.Id );
        }
    }
}
