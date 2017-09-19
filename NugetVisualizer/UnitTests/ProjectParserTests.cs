using System;
using System.Collections.Generic;
using System.Text;
using TestStack.BDDfy;

namespace UnitTests
{
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Moq;
    using Moq.AutoMock;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Exceptions;
    using NugetVisualizer.Core.Repositories;

    using Xunit;

    public class ProjectParserTests
    {
        private ProjectParser _projectParser;

        private List<IProjectIdentifier> _projectIdentifiers;

        private AutoMocker _autoMocker;

        public ProjectParserTests()
        {
            _autoMocker = new AutoMocker();
            _projectParser = _autoMocker.CreateInstance<ProjectParser>();
            _autoMocker.GetMock<IPackageParser>()
                        .Setup(x => x.ParsePackages(It.IsAny<XDocument>()))
                        .Returns(new List<Package>());
            _autoMocker.GetMock<IPackageReader>()
                        .Setup(x => x.GetPackagesContentsAsync(It.IsAny<IProjectIdentifier>()))
                        .ReturnsAsync(new List<XDocument>());
            _projectIdentifiers = new List<IProjectIdentifier>()
                                      {
                                          new ProjectIdentifier("first", "firstpath"),
                                          new ProjectIdentifier("notwork", "notworkpath"),
                                          new ProjectIdentifier("second", "secondpath"),
                                          new ProjectIdentifier("third", "thirdpath"),
                                          new ProjectIdentifier("notwork2", "notwork2path"),
                                          new ProjectIdentifier("last", "lastpath")
                                      };
        }

        [Fact]

        public async Task GivenAProjectCannotBeParsed_WhenParsingProject_ThenLastSuccessfullParsedProjectIsSaved()
        {
            this.Given(x => x.GivenAProjectCannotBeParsed())
                .When(x => x.WhenParsingProject())
                .Then(x => x.ThenLastSuccessfullParsedProjectIsSaved())
                .And(x => x.ThenWorkingProjectsAreSaved())
                .BDDfy();
        }

        private void GivenAProjectCannotBeParsed()
        {
            _autoMocker.GetMock<IPackageReader>()
                       .Setup(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.Name.Contains("notwork"))))
                       .Throws<CannotGetPackagesContentsException>();
        }

        private async Task WhenParsingProject()
        {
            var result = await _projectParser.ParseProjectsAsync(_projectIdentifiers);
        }

        private void ThenLastSuccessfullParsedProjectIsSaved()
        {
            //throw new NotImplementedException();
        }

        private void ThenWorkingProjectsAreSaved()
        {
            _autoMocker.GetMock<IProjectRepository>().Verify(x => x.Add(It.Is<Project>(p => p.Name.Equals("first")), It.IsAny<IEnumerable<int>>()));
            _autoMocker.GetMock<IProjectRepository>().Verify(x => x.Add(It.Is<Project>(p => p.Name.Equals("nowork")), It.IsAny<IEnumerable<int>>()), Times.Never);
            _autoMocker.GetMock<IProjectRepository>().Verify(x => x.Add(It.Is<Project>(p => p.Name.Equals("second")), It.IsAny<IEnumerable<int>>()));
            _autoMocker.GetMock<IProjectRepository>().Verify(x => x.Add(It.Is<Project>(p => p.Name.Equals("third")), It.IsAny<IEnumerable<int>>()));
            _autoMocker.GetMock<IProjectRepository>().Verify(x => x.Add(It.Is<Project>(p => p.Name.Equals("nowork2")), It.IsAny<IEnumerable<int>>()), Times.Never);
            _autoMocker.GetMock<IProjectRepository>().Verify(x => x.Add(It.Is<Project>(p => p.Name.Equals("last")), It.IsAny<IEnumerable<int>>()));
        }
    }
}
