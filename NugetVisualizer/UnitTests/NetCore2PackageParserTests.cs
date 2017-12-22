namespace UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.PackageParser;

    using Shouldly;

    using TestStack.BDDfy;
    using Xunit;

    public class NetCore2PackageParserTests
    {
        private NetCore2PackageParser _packageParser;

        private XDocument xmlDocument;

        private IEnumerable<Package> _results;

        public NetCore2PackageParserTests()
        {
            _packageParser = new NetCore2PackageParser();
        }


        [Fact]

        public void GivenAnEmptyXmlFile_WhenParsingXml_ThenEmptyResultListReturned()
        {
            this.Given(x => x.GivenAnEmptyXmlFile())
                .When(x => x.WhenParsingXml())
                .Then(x => x.ThenEmptyResultListReturned())
                .BDDfy();
        }

        [Fact]

        public void GivenAnXmlFileWithOnePackage_WhenParsingXml_ThenOnePackageIsReturned()
        {
            this.Given(x => x.GivenAnXmlFileWithOnePackage())
                .When(x => x.WhenParsingXml())
                .Then(x => x.ThenOnePackageIsReturned())
                .BDDfy();
        }

        [Fact]

        public void GivenAnXmlFileWithThreePackages_WhenParsingXml_ThenThreePackagesAreReturned()
        {
            this.Given(x => x.GivenAnXmlFileWithThreePackages())
                .When(x => x.WhenParsingXml())
                .Then(x => x.ThenThreePackagesAreReturned())
                .BDDfy();
        }

        private void GivenAnEmptyXmlFile()
        {
            xmlDocument = new XDocument();
        }

        private void GivenAnXmlFileWithOnePackage()
        {
            xmlDocument = XDocument.Parse("<Project Sdk=\"Microsoft.NET.Sdk.Web\">"
                                          + "<PropertyGroup>"
                                          + "<TargetFramework> netcoreapp2.0 </TargetFramework>"
                                          + "</PropertyGroup>"
                                          + "<ItemGroup >"
                                          + "<PackageReference Include=\"Microsoft.AspNetCore.All\" Version=\"2.0.0\" />"
                                          + "</ItemGroup>"
                                          + "</Project>");
        }

        private void GivenAnXmlFileWithThreePackages()
        {
            xmlDocument = XDocument.Parse("<Project Sdk=\"Microsoft.NET.Sdk.Web\">"
                                          + "<PropertyGroup>"
                                          + "<TargetFramework > netcoreapp2.0 </TargetFramework>"
                                          + "</PropertyGroup>"
                                          + "<ItemGroup>"
                                          + "<PackageReference Include=\"Newtonsoft.Json\" Version=\"9.0.1\" />"
                                          + "<PackageReference Include=\"EntityFramework\" Version=\"6.1.3\" />"
                                          + "<PackageReference Include=\"AutoMapper\" Version=\"3.3.1\" />"
                                          + "</ItemGroup>"
                                          + "</Project>");
        }

        private void WhenParsingXml()
        {
            _results = _packageParser.ParsePackages(xmlDocument);
        }

        private void ThenEmptyResultListReturned()
        {
            _results.ShouldBeEmpty();
        }

        private void ThenOnePackageIsReturned()
        {
            _results.Count().ShouldBe(1);
            var package = _results.Single();
            package.Name.ShouldBe("Microsoft.AspNetCore.All");
            package.Version.ShouldBe("2.0.0");
        }

        private void ThenThreePackagesAreReturned()
        {
            _results.Count().ShouldBe(3);
            _results.ElementAt(0).Name.ShouldBe("Newtonsoft.Json");
            _results.ElementAt(0).Version.ShouldBe("9.0.1");
            _results.ElementAt(1).Name.ShouldBe("EntityFramework");
            _results.ElementAt(1).Version.ShouldBe("6.1.3");
            _results.ElementAt(2).Name.ShouldBe("AutoMapper");
            _results.ElementAt(2).Version.ShouldBe("3.3.1");
        }
    }
}
