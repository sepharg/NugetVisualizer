namespace UnitTests
{
    using Autofac;

    using NugetVisualizer.Core.Repositories;

    public class DbTest : IntegrationTest
    {
        public DbTest()
        {
            InitializeTestDb();
        }

        private void InitializeTestDb()
        {
            using (var context = Container.Resolve<NugetVisualizerContext>())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}
