using System.Collections.Generic;

namespace WebVisualizer.Models
{
    using System.Linq;

    using Microsoft.AspNetCore.Mvc.Rendering;

    using NugetVisualizer.Core.Domain;

    public class PackagesViewModel
    {
        public PackagesViewModel()
        {
            PackagesOrderedByVersionCount = new List<SelectListItem>();
            Versions = new List<string>();
            ProjectRows = new List<ProjectRow>();
        }

        public void SetPackagesOrderedByVersionCount(Dictionary<Package, int> packages)
        {
            PackagesOrderedByVersionCount = packages.Select(p => new SelectListItem() { Text = p.Key.Name + $"({p.Value})", Value = p.Key.Name }).ToList();
        }

        public string SelectedPackageName { get; set; }
        
        public List<SelectListItem> PackagesOrderedByVersionCount { get; private set; }

        public List<string> Versions { get; set; }

        public List<ProjectRow> ProjectRows { get; set; }

        public List<Project> ProjectsForSelectedPackage { get; set; }
    }

    public class ProjectRow
    {
        public ProjectRow()
        {
            ValuesList = new List<bool>();
        }

        public string ProjectName { get; set; }

        public List<bool> ValuesList { get; set; }
    }
}
