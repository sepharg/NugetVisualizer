namespace NugetVisualizer
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    using NugetVisualizer.Domain;

    public class PackageParser : IPackageParser

    {
        public IEnumerable<Package> ParsePackages(XDocument packagesXml)
        {
            throw new NotImplementedException();
        }
    }
}
