namespace NugetVisualizer.Core.Domain
{
    using LiteDB;

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

        [BsonId]
        public string Name { get; set; }

        [BsonId]
        public string Version { get; set; }

        public string TargetFramework { get; set; }
    }
}
