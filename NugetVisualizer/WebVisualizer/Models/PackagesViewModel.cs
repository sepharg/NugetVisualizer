namespace WebVisualizer.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc.Rendering;

    using NugetVisualizer.Core.Domain;

    public class PackagesViewModel
    {
        public PackagesViewModel()
        {
            PackagesOrderedByVersionCount = new List<SelectListItem>();
            Projects = new List<SelectListItem>();
            Versions = new List<string>();
            ProjectRows = new List<ProjectRow>();
        }

        public void SetDropdowns(Dictionary<Package, int> packagesOrderedByVersionCount, List<Project> projects, List<Snapshot> snapshots)
        {
            PackagesOrderedByVersionCount = packagesOrderedByVersionCount.Select(p => new SelectListItem() { Text = p.Key.Name + $"({p.Value})", Value = p.Key.Name }).ToList();
            Projects = projects.Select(p => new SelectListItem() { Text = p.Name, Value = p.Name }).ToList();
            Snapshots = snapshots.Select(s => new SelectListItem() { Text = s.Name, Value = s.Version.ToString() }).ToList();
        }

        public string SelectedPackageName { get; set; }
        public string SelectedProjectName { get; set; }

        public int SelectedSnapshotId { get; set; }
        
        public List<SelectListItem> PackagesOrderedByVersionCount { get; private set; }
        public List<SelectListItem> Projects { get; private set; }

        public List<SelectListItem> Snapshots { get; private set; }

        public List<string> Versions { get; set; }

        public List<ProjectRow> ProjectRows { get; set; }
        
        // ToDo : probably split this viewmodel into 2! one for package search and another for project search
        public List<Package> PackagesForSelectedProject { get; set; }
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
