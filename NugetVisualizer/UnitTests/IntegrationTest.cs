namespace UnitTests
{
    using System.Collections.Generic;

    using Autofac;

    using Boostrapper;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Repositories;

    public class IntegrationTest
    {
        protected IContainer Container;

        protected Dictionary<string, string> IntegrationTestConfiguration { get; }

        public IntegrationTest()
        {
            var containerBuilder = AutofacContainerFactory.GetBuilder();
            var testConfigurationHelper = new TestConfigurationHelper();
            IntegrationTestConfiguration = testConfigurationHelper.IntegrationTestConfiguration;
            containerBuilder.RegisterInstance(testConfigurationHelper).As<IConfigurationHelper>();

            Container = containerBuilder.Build();
        }
    }
}
