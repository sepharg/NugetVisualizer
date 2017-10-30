namespace WebVisualizer.IoC
{
    using Autofac;

    using WebVisualizer.Services;

    public class WebModuleInstaller : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PackageSearchService>();
            builder.RegisterType<DashboardService>();
            builder.RegisterType<SnapshotService>();
        }
    }
}
