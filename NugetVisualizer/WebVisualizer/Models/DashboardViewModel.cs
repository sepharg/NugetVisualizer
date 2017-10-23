namespace WebVisualizer.Models
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class DashboardViewModel
    {
        public int SelectedSnapshotId { get; set; }

        public List<SelectListItem> Snapshots { get; set; }

        public MostUsedPackagesViewModel MostUsedPackagesViewModel { get; set; }
        public LeastUsedPackagesViewModel LeastUsedPackagesViewModel { get; set; }
    }
}
