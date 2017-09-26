namespace Boostrapper.Modules
{
    using System;

    using Autofac;
    using Autofac.Core;
    using Autofac.Core.Activators.Reflection;

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

            builder.Register<IProjectParser>(
                (context, parameters) =>
                    {
                        switch (parameters.TypedAs<ProjectParserType>())
                        {
                            case ProjectParserType.FileSystem:
                                return context.Resolve<ProjectParser>(
                                    new ResolvedParameter(
                                        (pi, ctx) => pi.ParameterType == typeof(IPackageReader),
                                        (pi, ctx) => ctx.Resolve<FileSystemPackageReader>()),
                                    new AutowiringParameter(),
                                    new AutowiringParameter(),
                                    new AutowiringParameter(),
                                    new AutowiringParameter());
                            case ProjectParserType.Github:
                                return context.Resolve<ProjectParser>(
                                    new ResolvedParameter(
                                        (pi, ctx) => pi.ParameterType == typeof(IPackageReader),
                                        (pi, ctx) => ctx.Resolve<GithubPackageReader>()),
                                    new AutowiringParameter(),
                                    new AutowiringParameter(),
                                    new AutowiringParameter(),
                                    new AutowiringParameter());
                            default: throw new ArgumentOutOfRangeException("Specified Project parser doesn't exist");
                        }
                    });
                   
            builder.RegisterType<ProjectParser>();

            builder.RegisterType<FileSystemPackageReader>();
            builder.RegisterType<GithubPackageReader>();

            builder.RegisterType<PackageRepository>().As<IPackageRepository>();
            builder.RegisterType<ProjectRepository>().As<IProjectRepository>();
            builder.RegisterType<ProjectParsingStateRepository>().As<IProjectParsingState>();


            builder.RegisterType<NugetVisualizerContext>().As<DbContext>().InstancePerLifetimeScope();
            builder.RegisterType<NugetVisualizerContext>().InstancePerLifetimeScope();

            builder.RegisterType<ConfigurationHelper>().As<IConfigurationHelper>();
        }
    }
}
