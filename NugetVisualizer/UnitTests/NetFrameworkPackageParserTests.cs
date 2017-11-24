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

    public class NetFrameworkPackageParserTests
    {
        private NugetVisualizer.Core.PackageParser.NetFrameworkPackageParser _packageParser;

        private XDocument xmlDocument;

        private IEnumerable<Package> _results;

        public NetFrameworkPackageParserTests()
        {
            _packageParser = new NetFrameworkPackageParser();
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
            xmlDocument = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-8\"?><packages><package id = \"Newtonsoft.Json\" version = \"9.0.1\" targetFramework = \"net461\" /></packages >");
        }

        private void GivenAnXmlFileWithThreePackages()
        {
            xmlDocument = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-8\"?><packages>"
                                          + "<package id = \"Newtonsoft.Json\" version = \"9.0.1\" targetFramework = \"net461\" />"
                                          + "<package id=\"EntityFramework\" version=\"6.1.3\" targetFramework=\"net462\" />"
                                          + "<package id=\"AutoMapper\" version=\"3.3.1\" />"
                                          + "</packages>");
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
            package.Name.ShouldBe("Newtonsoft.Json");
            package.Version.ShouldBe("9.0.1");
            package.TargetFramework.ShouldBe("net461");
        }

        private void ThenThreePackagesAreReturned()
        {
            _results.Count().ShouldBe(3);
            _results.ElementAt(0).Name.ShouldBe("Newtonsoft.Json");
            _results.ElementAt(0).Version.ShouldBe("9.0.1");
            _results.ElementAt(0).TargetFramework.ShouldBe("net461");
            _results.ElementAt(1).Name.ShouldBe("EntityFramework");
            _results.ElementAt(1).Version.ShouldBe("6.1.3");
            _results.ElementAt(1).TargetFramework.ShouldBe("net462");
            _results.ElementAt(2).Name.ShouldBe("AutoMapper");
            _results.ElementAt(2).Version.ShouldBe("3.3.1");
            _results.ElementAt(2).TargetFramework.ShouldBeNullOrEmpty();
        }
    }
}
