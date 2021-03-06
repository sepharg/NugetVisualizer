﻿namespace UnitTests
{
    using System;

    using Autofac;

    using Microsoft.EntityFrameworkCore;

    using NugetVisualizer.Core.Repositories;

    public class InMemoryDbTest : IntegrationTest
    {
        private DbContextOptions<NugetVisualizerContext> _dbContextOptions;
        
        protected override void ExtraRegistrations(ContainerBuilder builder)
        {
            _dbContextOptions = new DbContextOptionsBuilder<NugetVisualizerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            builder.RegisterInstance(_dbContextOptions).As<DbContextOptions<NugetVisualizerContext>>();
            builder.RegisterType<NugetVisualizerContext>().As<DbContext>().InstancePerLifetimeScope();
        }
    }
}