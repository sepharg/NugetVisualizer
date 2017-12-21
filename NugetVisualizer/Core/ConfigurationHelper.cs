namespace NugetVisualizer.Core
{
    using Microsoft.Extensions.Configuration;
    using System.Linq;

    public class ConfigurationHelper : IConfigurationHelper
    {
        private IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("configuration.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
        private IConfigurationSection GetSection(string sectionName)
        {
            var conf = GetConfiguration();
            return conf?.GetChildren().FirstOrDefault(t => t.Key.ToLower().Equals(sectionName.ToLower()));
        }

        public bool UseSqlLite => true;
        public string Dbpath => GetSection("Dbpath").Exists() ? GetSection("Dbpath").Value : "..\\nugetvisualizer.db";
        public string GithubToken => GetSection("GithubToken").Exists() ? GetSection("GithubToken").Value : "";
        public string GithubOrganization => GetSection("GithubOrganization").Exists() ? GetSection("GithubOrganization").Value : "";
    }

    public interface IConfigurationHelper
    {
        bool UseSqlLite { get; }
        string Dbpath { get; }
        string GithubToken { get; }
        string GithubOrganization { get; }
    }
}
