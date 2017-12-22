namespace NugetVisualizer.Core.FileSystem
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.PackageParser;

    public class FileSystemPackageReader : IPackageReader
    {
        private List<IPackageContainer> GetPackagesContents(IProjectIdentifier projectIdentifier)
        {
            return new List<IPackageContainer>(
                GetNetFrameworkPackagesFiles(projectIdentifier.Path)
                    .Select(
                        packagesFile =>
                            {
                                {
                                    using (var fs = new FileStream(packagesFile, FileMode.Open, FileAccess.Read))
                                    {
                                        return new NetFrameworkPackageContainer(XDocument.Load(fs));
                                    }
                                }
                            })
                    .Union<IPackageContainer>(
                        GetNetCore2PackagesFiles(projectIdentifier.Path)
                            .Select(
                                packagesFile =>
                                    {
                                        {
                                            using (var fs = new FileStream(packagesFile, FileMode.Open, FileAccess.Read))
                                            {
                                                return new NetCore2PackageContainer(XDocument.Load(fs));
                                            }
                                        }
                                    })
                            .ToList()));
        }

        public Task<List<IPackageContainer>> GetPackagesContentsAsync(IProjectIdentifier projectIdentifier)
        {
            return Task.FromResult(GetPackagesContents(projectIdentifier));
        }

        private string[] GetNetFrameworkPackagesFiles(string projectIdentifierPath)
        {
            var packagesFilePaths = Directory.GetFiles(projectIdentifierPath, "packages.config", SearchOption.AllDirectories);
            return packagesFilePaths;
        }

        private string[] GetNetCore2PackagesFiles(string projectIdentifierPath)
        {
            var packagesFilePaths = Directory.GetFiles(projectIdentifierPath, "*.csproj", SearchOption.AllDirectories);
            return packagesFilePaths;
        }
    }
}
