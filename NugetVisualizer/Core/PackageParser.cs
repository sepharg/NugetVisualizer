namespace NugetVisualizer.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using NugetVisualizer.Core.Domain;

    public class PackageParser : IPackageParser
    {
        public IEnumerable<Package> ParsePackages(XDocument packagesXml)
        {
            return Enumerable.Empty<Package>();
        }
    }
}
