namespace WebVisualizer.Models
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SearchPackagesViewModel
    {
        public SearchPackagesViewModel()
        {
            PackagesOrderedByVersionCount = new List<SelectListItem>();
            Versions = new List<string>();
            ProjectRows = new List<ProjectRow>();
        }

        public List<SelectListItem> PackagesOrderedByVersionCount { get; set; }

        public string SelectedPackageName { get; set; }

        public List<ProjectRow> ProjectRows { get; set; }

        public List<string> Versions { get; set; }
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
