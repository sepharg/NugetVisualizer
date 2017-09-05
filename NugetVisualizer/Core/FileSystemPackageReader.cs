namespace NugetVisualizer.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;

    public class FileSystemPackageReader : IPackageReader
    {
        public IEnumerable<XDocument> GetPackagesContents(IProjectIdentifier projectIdentifier)
        {
            // get all packages.json files

            foreach (var packagesFile in GetPackagesFiles(projectIdentifier.Path))
            {
                yield return XDocument.Load(new FileStream(packagesFile, FileMode.Open));
            }
        }

        private string[] GetPackagesFiles(string projectIdentifierPath)
        {
            var packagesFilePaths = Directory.GetFiles(projectIdentifierPath, "packages.json", SearchOption.AllDirectories);
            return packagesFilePaths;
        }
    }
}
