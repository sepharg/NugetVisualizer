namespace NugetVisualizer.Core.Domain
{
    using System.Collections.Generic;


    public class Project
    {
        public Project()
        {
            ProjectPackages = new List<ProjectPackage>();
        }

        public Project(string name) : this()
        {
            Name = name;
        }

        public ICollection<ProjectPackage> ProjectPackages { get; set; }

        public string Name { get; set; }
    }
}
