namespace NugetVisualizer.Core.PackageParser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;

    public class NetCore2PackageParser : IPackageParser
    {
        public IEnumerable<Package> ParsePackages(XDocument packagesXml)
        {
            if (packagesXml?.Root == null || !packagesXml.Root.Elements().Any())
            {
                return Enumerable.Empty<Package>();
            }

            var packages = from package in packagesXml.Root.Descendants("PackageReference")
                           select new Package(
                               package.Attribute("Include").Value,
                               package.Attribute("Version").Value,
                               string.Empty);
            return packages;
        }
    }
}
