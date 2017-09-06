namespace NugetVisualizer.Core.Domain
{
    using System.Collections.Generic;

    public class Project
    {
        public Project(string name)
        {
            Name = name;
            Packages = new List<Package>();
        }

        public List<Package> Packages { get; }

        public string Name { get; }
    }
}
