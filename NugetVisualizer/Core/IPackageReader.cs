namespace NugetVisualizer.Core
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;

    public interface IPackageReader
    {
        IEnumerable<XDocument> GetPackagesContents(IProjectIdentifier projectIdentifier);
    }
}