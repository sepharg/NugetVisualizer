namespace UnitTests
{
    using System.Collections.Generic;

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

        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"configuration.json", optional: true, reloadOnChange: true)
                                                    .AddInMemoryCollection(IntegrationTestConfiguration);
            return builder.Build();
        }

        public bool UseSqlLite => _useSqlLite;
    }
}
