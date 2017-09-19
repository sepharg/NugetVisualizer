namespace UnitTests
{
    using System;

    using Autofac;

    using NugetVisualizer.Core.Repositories;

    public class DbTest : IntegrationTest, IDisposable
    {
        protected string TempTestDbName;

        private NugetVisualizerContext _nugetVisualizerContext;

        public IContainer Container => base.Container;

        public DbTest()
        {
            TempTestDbName = $"integrationTestDb_{DateTime.Now.Ticks}";
            IntegrationTestConfiguration.Add("Dbpath", TempTestDbName);
            InitializeTestDb();
        }

        private void InitializeTestDb()
        {
            _nugetVisualizerContext = Container.Resolve<NugetVisualizerContext>();
            //context.Database.EnsureDeleted();
            _nugetVisualizerContext.Database.EnsureCreatedAsync();

            /*
            using (var context = Container.Resolve<NugetVisualizerContext>())
            {
                context.Database.EnsureCreated();
            }*/
        }
        public void Dispose()
        {
            _nugetVisualizerContext.Database.EnsureDeletedAsync();
            _nugetVisualizerContext?.Dispose();
        }
    }
}
