using System;

namespace Boostrapper
{
    using System.Reflection;

    using Autofac;

    public class AutofacContainerFactory
    {
        public static ContainerBuilder GetBuilder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(AutofacContainerFactory).GetTypeInfo().Assembly);
            return builder;
        }
    }
}
