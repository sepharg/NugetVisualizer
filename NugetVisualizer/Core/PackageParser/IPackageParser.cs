namespace NugetVisualizer.Core.PackageParser
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;

    public interface IPackageParser
    {
        IEnumerable<Package> ParsePackages(XDocument packageContainer);
    }
}
