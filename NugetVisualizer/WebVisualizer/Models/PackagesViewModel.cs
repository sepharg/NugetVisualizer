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
            Packages = new List<SelectListItem>();
            Versions = new List<string>();
            ProjectRows = new List<ProjectRow>();
        }

        public PackagesViewModel(List<Package> packages) : this()
        {
            SetPackages(packages);
        }

        public void SetPackages(List<Package> packages)
        {
            Packages = packages.Select(p => new SelectListItem() { Text = p.Name, Value = p.Name }).ToList();
        }

        public string SelectedPackageName { get; set; }

        public List<SelectListItem> Packages { get; private set; }

        public List<string> Versions { get; set; }

        public List<ProjectRow> ProjectRows { get; set; }

        public List<Project> ProjectsForSelectedPackage { get; set; }
    }

    public class ProjectRow
    {
        public ProjectRow()
        {
            ValuesList = new List<string>();
        }

        public string ProjectName { get; set; }

        public List<string> ValuesList { get; set; }
    }
}
