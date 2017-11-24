namespace NugetVisualizer.Core.PackageParser
{
    using System;

    public class PackageParserFactory : IPackageParserFactory
    {
        public IPackageParser GetPackageParser(PackageType type)
        {
            switch (type)
            {
                case PackageType.NetFramework:
                    {
                        return new NetFrameworkPackageParser();
                    }
                case PackageType.NetCore2:
                    {
                        return new NetCore2PackageParser();
                    }
                default: throw new ArgumentException($"Cannot resolve a package parser for {type} PackageType");
            }
        }
    }
}
