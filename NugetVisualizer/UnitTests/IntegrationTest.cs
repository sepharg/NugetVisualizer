namespace UnitTests
{
    using System.Collections.Generic;

    using Autofac;

    using Boostrapper;

    using NugetVisualizer.Core;

    public abstract class IntegrationTest
    {
        protected IContainer Container;

        protected Dictionary<string, string> IntegrationTestConfiguration { get; }

        public IntegrationTest()
        {
            var containerBuilder = AutofacContainerFactory.GetBuilder();
            var testConfigurationHelper = new TestConfigurationHelper();
            IntegrationTestConfiguration = testConfigurationHelper.IntegrationTestConfiguration;
            ExtraRegistrations(containerBuilder);
            containerBuilder.RegisterInstance(testConfigurationHelper).As<IConfigurationHelper>();

            Container = containerBuilder.Build();
        }

        protected abstract void ExtraRegistrations(ContainerBuilder builder);
    }
}
