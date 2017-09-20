namespace UnitTests
{
    using System.Collections.Generic;

    using Microsoft.Extensions.Configuration;

    using NugetVisualizer.Core;

    public class TestConfigurationHelper : IConfigurationHelper
    {
        public Dictionary<string, string> IntegrationTestConfiguration { get; }

        public TestConfigurationHelper()
        {
            IntegrationTestConfiguration = new Dictionary<string, string>();
        }

        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"configuration.json", optional: true, reloadOnChange: true)
                                                    .AddInMemoryCollection(IntegrationTestConfiguration);
            return builder.Build();
        }
    }
}
