namespace NugetVisualizer.Core.Domain
{

    public class Package
    {
        public Package()
        {
        }

        public Package(string name, string version, string targetFramework)
        {
            Name = name;
            Version = version;
            TargetFramework = targetFramework;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public string TargetFramework { get; set; }
    }
}
