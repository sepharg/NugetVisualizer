namespace NugetVisualizer.Core.PackageParser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;

    public class NetFrameworkPackageParser : IPackageParser
    {
        public IEnumerable<Package> ParsePackages(XDocument packagesXml)
        {
            if (packagesXml?.Root == null || !packagesXml.Root.Elements().Any())
            {
                return Enumerable.Empty<Package>();
            }

            var packages = from package in packagesXml.Root.Descendants("package")
                           select new Package(
                               package.Attribute("id").Value,
                               package.Attribute("version").Value,
                               package.Attribute("targetFramework")?.Value);
            return packages;
        }
    }
}
