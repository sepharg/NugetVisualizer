namespace NugetVisualizer.Core.Domain
{
    using System.Collections.Generic;

    using LiteDB;

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

        [BsonId]
        public string Name { get; set; }
    }
}
