using System;
using System.Collections.Generic;
using System.Text;

namespace NugetVisualizer.Core.Repositories
{
    using Microsoft.EntityFrameworkCore.Design;

    public class NugetVisualizerContextFactory : IDesignTimeDbContextFactory<NugetVisualizerContext>
    {
        public NugetVisualizerContext CreateDbContext(string[] args)
        {
            return new NugetVisualizerContext(new ConfigurationHelper());
        }
    }
}
