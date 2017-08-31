using System;

namespace NugetVisualizer
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    using NugetVisualizer.Domain;

    public interface IPackageParser
    {
        IEnumerable<Package> ParsePackages(XDocument packagesXml);
    }
}
