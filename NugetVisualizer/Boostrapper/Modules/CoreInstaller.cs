namespace Boostrapper.Modules
{
    using Autofac;

    using Microsoft.EntityFrameworkCore;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.FileSystem;
    using NugetVisualizer.Core.Github;
    using NugetVisualizer.Core.Repositories;

    public class CoreInstaller : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PackageParser>().As<IPackageParser>();

            builder.RegisterType<FileSystemRepositoryReader>();
            builder.RegisterType<GithubRepositoryReader>();

            builder.RegisterType<FileSystemProjectParser>();
            builder.RegisterType<GithubProjectParser>();

            builder.RegisterType<FileSystemPackageReader>();
            builder.RegisterType<GithubPackageReader>();

            builder.RegisterType<PackageRepository>();
            builder.RegisterType<ProjectRepository>();

            builder.RegisterType<NugetVisualizerContext>().As<DbContext>().InstancePerLifetimeScope();
            builder.RegisterType<NugetVisualizerContext>().InstancePerLifetimeScope();

            builder.RegisterType<ConfigurationHelper>().As<IConfigurationHelper>();
        }
    }
}
