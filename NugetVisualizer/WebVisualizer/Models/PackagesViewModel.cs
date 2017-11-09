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
            SearchPackagesViewModel = new SearchPackagesViewModel();
            SearchProjectsViewModel = new SearchProjectsViewModel();
        }

        public void SetDropdowns(Dictionary<Package, int> packagesOrderedByVersionCount, List<Project> projects, List<Snapshot> snapshots)
        {
            SearchPackagesViewModel.PackagesOrderedByVersionCount = packagesOrderedByVersionCount.Select(p => new SelectListItem() { Text = p.Key.Name + $"({p.Value})", Value = p.Key.Name }).ToList();
            SearchProjectsViewModel.Projects = projects.Select(p => new SelectListItem() { Text = p.Name, Value = p.Name }).ToList();
            Snapshots = snapshots.Select(s => new SelectListItem() { Text = s.Name, Value = s.Version.ToString() }).ToList();
        }

        public SearchPackagesViewModel SearchPackagesViewModel { get; set; }

        public SearchProjectsViewModel SearchProjectsViewModel { get; set; }

        public int SelectedSnapshotId { get; set; }

        public List<SelectListItem> Snapshots { get; private set; }
    }
}
