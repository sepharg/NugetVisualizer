namespace NugetVisualizer.Core
{
    using Microsoft.Extensions.Configuration;

    public class ConfigurationHelper : IConfigurationHelper
    {
        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("configuration.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
    }

    public interface IConfigurationHelper
    {
        IConfigurationRoot GetConfiguration();
    }
}
