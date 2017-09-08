namespace NugetVisualizer.Core
{
    using System;

    using Microsoft.Extensions.Configuration;

    public class ConfigurationHelper
    {
        public IConfigurationRoot GetConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("NUGET_VISUALIZER_ENVIRONMENT");
            if (string.IsNullOrEmpty(environmentName))
            {
                environmentName = "Development"; // ToDo: workaround until i find what's going on with the environment variable not being set
            }
            var builder = new ConfigurationBuilder().AddJsonFile($"configuration.{environmentName}.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
    }
}
