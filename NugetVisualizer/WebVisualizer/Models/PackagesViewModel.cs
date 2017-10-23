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

        public void SetPackagesOrderedByVersionCount(Dictionary<Package, int> packages, List<Snapshot> snapshots)
        {
            PackagesOrderedByVersionCount = packages.Select(p => new SelectListItem() { Text = p.Key.Name + $"({p.Value})", Value = p.Key.Name }).ToList();
            Snapshots = snapshots.Select(s => new SelectListItem() { Text = s.Name, Value = s.Version.ToString() }).ToList();
        }

        public string SelectedPackageName { get; set; }

        public int SelectedSnapshotId { get; set; }
        
        public List<SelectListItem> PackagesOrderedByVersionCount { get; private set; }

        public List<SelectListItem> Snapshots { get; private set; }

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
