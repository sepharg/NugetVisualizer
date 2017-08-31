namespace UnitTests
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;

    using Shouldly;

    using TestStack.BDDfy;
    using Xunit;

    public class PackageParserTests
    {
        private PackageParser _packageParser;

        private XDocument xmlDocument;

        private IEnumerable<Package> _results;

        public PackageParserTests()
        {
            _packageParser = new NugetVisualizer.Core.PackageParser();
        }


        [Fact]

        public void GivenAnEmptyXmlFile_WhenParsingXml_ThenEmptyResultListReturned()
        {
            this.Given(x => x.GivenAnEmptyXmlFile())
                .When(x => x.WhenParsingXml())
                .Then(x => x.ThenEmptyResultListReturned())
                .BDDfy();
        }

        private void GivenAnEmptyXmlFile()
        {
            xmlDocument = new XDocument();
        }

        private void WhenParsingXml()
        {
            _results = _packageParser.ParsePackages(xmlDocument);
        }

        private void ThenEmptyResultListReturned()
        {
            _results.ShouldBeEmpty();
        }
    }
}
