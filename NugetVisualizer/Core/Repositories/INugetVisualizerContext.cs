namespace NugetVisualizer.Core.Repositories
{
    using Microsoft.EntityFrameworkCore;

    using NugetVisualizer.Core.Domain;

    public interface INugetVisualizerContext
    {
        DbSet<Project> Projects { get; set; }

        DbSet<Package> Packages { get; set; }

        DbSet<ProjectPackage> ProjectPackages { get; set; }

        int SaveChanges();

        void Dispose();
    }
}