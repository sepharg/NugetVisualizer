namespace NugetVisualizer.Core.PackageParser
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;

    public class NetCore2PackageParser  : IPackageParser
    {
        public IEnumerable<Package> ParsePackages(XDocument packageContainer)
        {
            throw new NotImplementedException();
        }
    }
}
