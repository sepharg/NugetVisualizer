namespace NugetVisualizer.Core.Domain
{
    using System.Collections.Generic;


    public class Project
    {
        public Project()
        {
        }

        public Project(string name)
        {
            Name = name;
            Packages = new List<Package>();
        }

        public List<Package> Packages { get; set; }

        public string Name { get; set; }
    }
}
