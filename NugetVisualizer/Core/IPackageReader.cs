namespace NugetVisualizer.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;

    public interface IPackageReader
    {
        Task<List<XDocument>> GetPackagesContentsAsync(IProjectIdentifier projectIdentifier);
    }
}