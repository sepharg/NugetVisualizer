namespace NugetVisualizer.Core.Domain
{
    using System.Collections.Generic;

    public class Project : IProjectIdentifier
    {
        public Project(string name, string path)
        {
            Name = name;
            Path = path;
            Packages = new List<Package>();
        }

        public List<Package> Packages { get; }

        public string Name { get; }

        public string Path { get; }
    }
}
