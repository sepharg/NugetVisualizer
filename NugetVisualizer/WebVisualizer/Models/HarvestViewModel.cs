namespace WebVisualizer.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Dto;

    public class HarvestViewModel
    {
        public string GithubOrganization { get; set; }

        public HarvestViewModel()
        {
        }

        public HarvestViewModel(string githubOrganization)
        {
            GithubOrganization = githubOrganization;
        }

        public int SelectedSnapshotId { get; set; }
        public List<SelectListItem> Snapshots { get; set; }

        public ProjectParserType ParserType { get; set; }

        [Required]
        public string RootPath { get; set; }

        [Required]
        public string SnapshotName { get; set; }

        public bool Append { get; set; }

        public string Filters { get; set; }

        public ProjectParsingResult ProjectParsingResult { get; set; }

        public List<SelectListItem> ParserTypes { get
            {
                return new List<SelectListItem>()
                           {
                               new SelectListItem()
                                   {
                                       Text = "File System",
                                       Value = ProjectParserType.FileSystem.ToString()
                                   },
                               new SelectListItem()
                                   {
                                       Text = "Github",
                                       Value = ProjectParserType.Github.ToString()
                                   }
                           };
            }
        }
    }
}
