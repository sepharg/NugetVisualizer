namespace NugetVisualizer.Core.PackageParser
{
    using System.Xml.Linq;

    public interface IPackageContainer
    {
        PackageType PackageType { get; }

        XDocument PackageContents { get; }
    }

    public class NetFrameworkPackageContainer : IPackageContainer
    {
        public PackageType PackageType => PackageType.NetFramework;

        public XDocument PackageContents { get; }

        public NetFrameworkPackageContainer(XDocument contents)
        {
            PackageContents = contents;
        }
    }

    public class NetCore2PackageContainer : IPackageContainer
    {
        public PackageType PackageType => PackageType.NetCore2;

        public XDocument PackageContents { get; }

        public NetCore2PackageContainer(XDocument contents)
        {
            PackageContents = contents;
        }
    }
}
