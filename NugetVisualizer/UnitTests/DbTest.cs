﻿namespace UnitTests
{
    using System;

    using Autofac;

    using NugetVisualizer.Core.Repositories;

    public class DbTest : IntegrationTest, IDisposable
    {
        protected string TempTestDbName;

        private NugetVisualizerContext _nugetVisualizerContext;

        public IContainer Container => base.Container;

        public DbTest() : base(new TestConfigurationHelper(true))
        {
            TempTestDbName = $"integrationTestDb_{Guid.NewGuid()}";
            IntegrationTestConfiguration.Add("Dbpath", TempTestDbName);
            InitializeTestDb();
        }

        private void InitializeTestDb()
        {
            _nugetVisualizerContext = Container.Resolve<NugetVisualizerContext>();
            _nugetVisualizerContext.Database.EnsureCreatedAsync();
        }
        public void Dispose()
        {
            _nugetVisualizerContext.Database.EnsureDeletedAsync();
            _nugetVisualizerContext?.Dispose();
        }

        protected override void ExtraRegistrations(ContainerBuilder builder)
        {
        }
    }
}
