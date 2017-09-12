namespace NugetVisualizer.Core.Domain
{
    using System.Collections.Generic;

    public class Package
    {
        public Package()
        {
            ProjectPackages = new List<ProjectPackage>();
        }

        public Package(string name, string version, string targetFramework) : this()
        {
            Name = name;
            Version = version;
            TargetFramework = targetFramework;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public string TargetFramework { get; set; }

        public ICollection<ProjectPackage> ProjectPackages { get; set; }
    }
}
