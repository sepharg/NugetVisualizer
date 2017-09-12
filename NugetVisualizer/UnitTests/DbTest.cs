namespace UnitTests
{
    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Repositories;

    public class DbTest
    {
        protected ConfigurationHelper _configurationHelper;

        public DbTest()
        {
            _configurationHelper = new ConfigurationHelper();
            InitializeTestDb();
        }

        private void InitializeTestDb()
        {
            using (var context = new NugetVisualizerContext(_configurationHelper))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}
