namespace NugetVisualizer.Core.FileSystem
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;

    public class FileSystemPackageReader : IPackageReader
    {
        public List<XDocument> GetPackagesContents(IProjectIdentifier projectIdentifier)
        {
            return Enumerable.Select(GetPackagesFiles(projectIdentifier.Path), packagesFile => XDocument.Load(new FileStream(packagesFile, FileMode.Open))).ToList();
        }

        public Task<List<XDocument>> GetPackagesContentsAsync(IProjectIdentifier projectIdentifier)
        {
            return Task.FromResult<List<XDocument>>(GetPackagesContents(projectIdentifier));
        }

        private string[] GetPackagesFiles(string projectIdentifierPath)
        {
            var packagesFilePaths = Directory.GetFiles(projectIdentifierPath, "packages.config", SearchOption.AllDirectories);
            return packagesFilePaths;
        }
    }
}
