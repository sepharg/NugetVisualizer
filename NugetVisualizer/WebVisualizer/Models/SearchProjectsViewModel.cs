namespace WebVisualizer.Models
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    using NugetVisualizer.Core.Domain;

    public class SearchProjectsViewModel
    {
        public SearchProjectsViewModel()
        {
            Projects = new List<SelectListItem>();
            PackagesForSelectedProject = new List<Package>();
        }

        public List<SelectListItem> Projects { get; set; }
        public string SelectedProjectName { get; set; }
        public List<Package> PackagesForSelectedProject { get; set; }
    }
}
