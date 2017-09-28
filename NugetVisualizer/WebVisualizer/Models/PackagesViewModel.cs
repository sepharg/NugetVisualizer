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
            Packages =
                new List<Package>()
                        {
                            new Package("First", "1.0", ""),
                            new Package("Second", "2.0", ""),
                            new Package("Third", "3.0", ""),
                        }.Select(p => new SelectListItem() { Text = p.Name, Value = p.Name })
                    .ToList();

            Versions = new List<string>();

            ProjectRows = new List<ProjectRow>();
        }

        public Package SelectedPackage { get; set; }

        public List<SelectListItem> Packages { get; }

        public List<string> Versions { get; set; }

        public List<ProjectRow> ProjectRows { get; set; }

        public List<Project> ProjectsForSelectedPackage { get; set; }
    }

    public class ProjectRow
    {
        public string ProjectName { get; set; }

        public List<string> ValuesList { get; set; }
    }
}
