namespace NugetVisualizer.Core.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    public class NugetVisualizerContextFactory : IDesignTimeDbContextFactory<NugetVisualizerContext>
    {
        public NugetVisualizerContext CreateDbContext(string[] args)
        {
            return new NugetVisualizerContext(new ConfigurationHelper(), new DbContextOptionsBuilder<NugetVisualizerContext>().Options);
        }
    }
}
