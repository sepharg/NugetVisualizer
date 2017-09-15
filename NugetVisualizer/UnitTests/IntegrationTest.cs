namespace UnitTests
{
    using Autofac;

    using Boostrapper;

    using NugetVisualizer.Core.Repositories;

    public class IntegrationTest
    {
        protected IContainer Container;

        public IntegrationTest()
        {
            Container = AutofacContainerFactory.GetBuilder().Build();
        }
    }
}
