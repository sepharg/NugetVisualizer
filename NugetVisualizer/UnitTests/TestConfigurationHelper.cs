namespace UnitTests
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Repositories;

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
            var conf = GetConfiguration();
            return conf?.GetChildren().FirstOrDefault(t => t.Key.ToLower().Equals(sectionName.ToLower()));
        }

        public bool UseSqlLite => _useSqlLite;
        public string Dbpath => GetSection("Dbpath").Exists() ? GetSection("Dbpath").Value : "";
        public string GithubToken => GetSection("GithubToken").Exists() ? GetSection("GithubToken").Value : "";
        public string GithubOrganization => GetSection("GithubOrganization").Exists() ? GetSection("GithubOrganization").Value : "";
    }
}
