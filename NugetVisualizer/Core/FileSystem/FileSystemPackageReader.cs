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
        private List<XDocument> GetPackagesContents(IProjectIdentifier projectIdentifier)
        {
            return GetPackagesFiles(projectIdentifier.Path)
                .Select(
                    packagesFile =>
                        {
                            {
                                using (var fs = new FileStream(packagesFile, FileMode.Open, FileAccess.Read))
                                {
                                    return XDocument.Load(fs);
                                }
                            }
                        })
                .ToList();
        }

        public Task<List<XDocument>> GetPackagesContentsAsync(IProjectIdentifier projectIdentifier)
        {
            return Task.FromResult(GetPackagesContents(projectIdentifier));
        }

        private string[] GetPackagesFiles(string projectIdentifierPath)
        {
            var packagesFilePaths = Directory.GetFiles(projectIdentifierPath, "packages.config", SearchOption.AllDirectories);
            return packagesFilePaths;
        }
    }
}
