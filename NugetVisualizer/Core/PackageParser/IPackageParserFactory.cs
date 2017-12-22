namespace NugetVisualizer.Core.PackageParser
{
    public interface IPackageParserFactory
    {
        IPackageParser GetPackageParser(PackageType type);
    }
}
