using System;
using System.Collections.Generic;
using System.Text;

namespace NugetVisualizer.Core.Domain
{
    public class ProjectPackage
    {
        public string ProjectName { get; set; }

        public Project Project { get; set; }

        public int PackageId { get; set; }

        public Package Package { get; set; }
    }
}
