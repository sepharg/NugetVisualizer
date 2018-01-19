namespace UnitTests
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Repositories;
    using System.Collections.Generic;
    using System.Linq;

    public class TestConfigurationHelper : IConfigurationHelper
    {
        private bool _useSqlLite;

        public Dictionary<string, string> IntegrationTestConfiguration { get; }

        public DbContextOptionsBuilder<NugetVisualizerContext> DbContextOptionsBuilder { get; set; }

        public TestConfigurationHelper(bool useSqlLite)
        {
            IntegrationTestConfiguration = new Dictionary<string, string>();
            _useSqlLite = useSqlLite;
        }

        private IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"configuration.json", optional: true, reloadOnChange: true)
                                                    .AddInMemoryCollection(IntegrationTestConfiguration);
            return builder.Build();
        }
        private IConfigurationSection GetSection(string sectionName)
        {
            return GetConfiguration()?.GetChildren().FirstOrDefault(t => t.Key.ToLower().Equals(sectionName.ToLower()));
        }

        public bool UseSqlLite => _useSqlLite;
        public string Dbpath => GetSection("Dbpath").Exists() ? GetSection("Dbpath").Value : ".." + System.IO.Path.DirectorySeparatorChar + "nugetvisualizer.db";
        public string GithubToken => GetSection("GithubToken").Exists() ? GetSection("GithubToken").Value : "";
        public string GithubOrganization => GetSection("GithubOrganization").Exists() ? GetSection("GithubOrganization").Value : "";
    }
}
