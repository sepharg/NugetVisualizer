namespace NugetVisualizer.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using NugetVisualizer.Core.Domain;

    public class PackageParser : IPackageParser
    {
        public IEnumerable<Package> ParsePackages(XDocument packagesXml)
        {
            if (packagesXml?.Root == null || !packagesXml.Root.Elements().Any())
            {
                return Enumerable.Empty<Package>();
            }

            var packages = from package in packagesXml.Root.Descendants("package")
                           select new Package()
                                      {
                                          Name = package.Attribute("id").Value,
                                          Version = package.Attribute("version").Value,
                                          TargetFramework = package.Attribute("targetFramework").Value
                                      };
            return packages;
        }
    }
}
