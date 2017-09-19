namespace UnitTests
{
    using System;
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
            var environmentName = Environment.GetEnvironmentVariable("NUGET_VISUALIZER_ENVIRONMENT");
            if (string.IsNullOrEmpty(environmentName))
            {
                environmentName = "Development"; // ToDo: workaround until i find what's going on with the environment variable not being set
            }
            var builder = new ConfigurationBuilder().AddJsonFile($"configuration.{environmentName}.json", optional: true, reloadOnChange: true).AddInMemoryCollection(IntegrationTestConfiguration);
            return builder.Build();
        }
    }
}
