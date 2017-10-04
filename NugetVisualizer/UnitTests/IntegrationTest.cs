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

        public IntegrationTest() : this(new TestConfigurationHelper(false))
        {
        }

        public IntegrationTest(TestConfigurationHelper configurationHelper)
        {
            var containerBuilder = AutofacContainerFactory.GetBuilder();
            IntegrationTestConfiguration = configurationHelper.IntegrationTestConfiguration;
            ExtraRegistrations(containerBuilder);
            containerBuilder.RegisterInstance(configurationHelper).As<IConfigurationHelper>();

            Container = containerBuilder.Build();
        }

        protected abstract void ExtraRegistrations(ContainerBuilder builder);
    }
}
