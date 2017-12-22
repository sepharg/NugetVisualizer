using System;
using System.Collections.Generic;
using System.Text;
using TestStack.BDDfy;

namespace UnitTests
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Moq;
    using Moq.AutoMock;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Dto;
    using NugetVisualizer.Core.Exceptions;
    using NugetVisualizer.Core.PackageParser;
    using NugetVisualizer.Core.Repositories;

    using Shouldly;

    using Xunit;

    public class ProjectParserTests
    {
        private ProjectParser _projectParser;

        private List<IProjectIdentifier> _projectIdentifiers;

        private AutoMocker _autoMocker;

        private ProjectParsingResult _projectParsingResult;

        private int _snapshotVersion = 5;

        public ProjectParserTests()
        {
            _autoMocker = new AutoMocker();
            _projectParser = _autoMocker.CreateInstance<ProjectParser>();
            _autoMocker.GetMock<IPackageParser>()
                        .Setup(x => x.ParsePackages(It.IsAny<XDocument>()))
                        .Returns(new List<Package>());
            _autoMocker.GetMock<IPackageReader>()
                        .Setup(x => x.GetPackagesContentsAsync(It.IsAny<IProjectIdentifier>()))
                        .ReturnsAsync(new List<IPackageContainer>());
            _projectIdentifiers = new List<IProjectIdentifier>()
                                      {
                                          new ProjectIdentifier("first", "repo", "firstpath"),
                                          new ProjectIdentifier("second", "repo", "secondpath"),
                                          new ProjectIdentifier("notwork", "repo", "notworkpath"),
                                          new ProjectIdentifier("third", "repo", "thirdpath"),
                                          new ProjectIdentifier("notwork2", "repo", "notwork2path"),
                                          new ProjectIdentifier("last", "repo", "lastpath")
                                      };
        }

        [Fact]

        public async Task GivenAProjectCannotBeParsedFatal_WhenParsingProject_ThenNotAllExistingProjectsCanBeParsed()
        {
            this.Given(x => x.GivenAProjectCannotBeParsedFatal())
                .When(x => x.WhenParsingProject())
                .Then(x => x.ThenNotAllExistingProjectsCanBeParsed())
                .And(x => x.ThenStopsAtFirstError())
                .BDDfy();
        }

        [Fact]

        public async Task GivenAProjectCannotBeParsedSoft_WhenParsingProject_ThenNotAllExistingProjectsCanBeParsed()
        {
            this.Given(x => x.GivenAProjectCannotBeParsedSoft())
                .When(x => x.WhenParsingProject())
                .Then(x => x.ThenNotAllExistingProjectsCanBeParsed())
                .And(x => x.ThenParsesWhatItCan())
                .And(x => x.ThenPartialParsingErrorsAreReturned())
                .BDDfy();
        }

        [Fact]

        public async Task GivenAProjectCannotBeParsedFatal_WhenParsingProject_ThenSavesProjectUntilItFails()
        {
            this.Given(x => x.GivenAProjectCannotBeParsedFatal())
                .When(x => x.WhenParsingProject())
                .Then(x => x.ThenLastSuccessfullParsedProjectIsSaved())
                .And(x => x.ThenWorkingProjectsAreSaved())
                .BDDfy();
        }

        [Fact]

        public async Task GivenFirstProjectCannotBeParsedFatal_WhenParsingProject_ThenNothingIsSaved()
        {
            this.Given(x => x.GivenFirstProjectCannotBeParsedFatal())
                .When(x => x.WhenParsingProject())
                .Then(x => x.ThenLastSuccessfullParsedProjectIsNotSaved())
                .BDDfy();
        }

        [Fact]

        public async Task GivenAllProjectsCanBeParsed_WhenParsingProject_ThenProjectParsingStateIsDeleted()
        {
            this.Given(x => x.GivenAllProjectsCanBeParsed())
                .When(x => x.WhenParsingProject())
                .Then(x => x.ThenProjectParsingStateIsDeleted())
                .BDDfy();
        }

        private void GivenAllProjectsCanBeParsed()
        {
            _autoMocker.GetMock<IPackageReader>()
                .Setup(x => x.GetPackagesContentsAsync(It.IsAny<IProjectIdentifier>()))
                .ReturnsAsync(new List<IPackageContainer>());
        }

        private void GivenAProjectCannotBeParsedFatal()
        {
            _autoMocker.GetMock<IPackageReader>()
                       .Setup(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Contains("notwork"))))
                       .Throws<CannotGetPackagesContentsException>(); // cannot get any packages contents from now on. i.e.: github api token limit exceeded.
        }

        private void GivenAProjectCannotBeParsedSoft()
        {
            _autoMocker.GetMock<IPackageReader>()
                .Setup(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Contains("notwork"))))
                .Throws<IOException>(); // file is accessed exclusively by another process. will skip
            _autoMocker.GetMock<IPackageReader>()
                .Setup(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Contains("notwork2"))))
                .Throws<IOException>(); // file is accessed exclusively by another process. will skip
        }

        private void GivenFirstProjectCannotBeParsedFatal()
        {
            _autoMocker.GetMock<IPackageReader>()
                .Setup(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Contains("first"))))
                .Throws<CannotGetPackagesContentsException>();
        }

        private async Task WhenParsingProject()
        {
            _projectParsingResult = await _projectParser.ParseProjectsAsync(_projectIdentifiers, _snapshotVersion);
        }

        private void ThenLastSuccessfullParsedProjectIsSaved()
        {
            _autoMocker.GetMock<IProjectParsingState>().Verify(x => x.SaveLatestParsedProject("second"));
        }

        private void ThenLastSuccessfullParsedProjectIsNotSaved()
        {
            _autoMocker.GetMock<IProjectParsingState>().Verify(x => x.SaveLatestParsedProject(It.IsAny<string>()), Times.Never);
        }

        private void ThenProjectParsingStateIsDeleted()
        {
            _autoMocker.GetMock<IProjectParsingState>().Verify(x => x.DeleteLatestParsedProject());
        }

        private void ThenStopsAtFirstError()
        {
            _autoMocker.GetMock<IPackageReader>().Verify(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Equals("first"))));
            _autoMocker.GetMock<IPackageReader>().Verify(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Equals("second"))));
            _autoMocker.GetMock<IPackageReader>().Verify(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Equals("notwork"))));
            _autoMocker.GetMock<IPackageReader>().Verify(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Equals("third"))), Times.Never);
            _autoMocker.GetMock<IPackageReader>().Verify(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Equals("notwork2"))), Times.Never);
            _autoMocker.GetMock<IPackageReader>().Verify(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Equals("last"))), Times.Never);
        }

        private void ThenParsesWhatItCan()
        {
            _autoMocker.GetMock<IPackageReader>().Verify(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Equals("first"))));
            _autoMocker.GetMock<IPackageReader>().Verify(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Equals("second"))));
            _autoMocker.GetMock<IPackageReader>().Verify(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Equals("notwork"))));
            _autoMocker.GetMock<IPackageReader>().Verify(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Equals("third"))));
            _autoMocker.GetMock<IPackageReader>().Verify(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Equals("notwork2"))));
            _autoMocker.GetMock<IPackageReader>().Verify(x => x.GetPackagesContentsAsync(It.Is<IProjectIdentifier>(pi => pi.SolutionName.Equals("last"))));
        }

        private void ThenNotAllExistingProjectsCanBeParsed()
        {
            _projectParsingResult.AllExistingProjectsParsed.ShouldBeFalse();
        }

        private void ThenPartialParsingErrorsAreReturned()
        {
            _projectParsingResult.ParsingErrors.Count.ShouldBe(2);
            _projectParsingResult.ParsingErrors.FirstOrDefault(x => x.Contains("notwork")).ShouldNotBeNull();
            _projectParsingResult.ParsingErrors.SingleOrDefault(x => x.Contains("notwork2")).ShouldNotBeNull();
        }

        private void ThenWorkingProjectsAreSaved()
        {
            _autoMocker.GetMock<IProjectRepository>().Verify(x => x.Add(It.Is<Project>(p => p.Name.Equals("first")), It.IsAny<IEnumerable<int>>(), _snapshotVersion));
            _autoMocker.GetMock<IProjectRepository>().Verify(x => x.Add(It.Is<Project>(p => p.Name.Equals("second")), It.IsAny<IEnumerable<int>>(), _snapshotVersion));
            _autoMocker.GetMock<IProjectRepository>().Verify(x => x.Add(It.Is<Project>(p => p.Name.Equals("notwork")), It.IsAny<IEnumerable<int>>(), _snapshotVersion), Times.Never);
            _autoMocker.GetMock<IProjectRepository>().Verify(x => x.Add(It.Is<Project>(p => p.Name.Equals("third")), It.IsAny<IEnumerable<int>>(), _snapshotVersion), Times.Never);
            _autoMocker.GetMock<IProjectRepository>().Verify(x => x.Add(It.Is<Project>(p => p.Name.Equals("notwork2")), It.IsAny<IEnumerable<int>>(), _snapshotVersion), Times.Never);
            _autoMocker.GetMock<IProjectRepository>().Verify(x => x.Add(It.Is<Project>(p => p.Name.Equals("last")), It.IsAny<IEnumerable<int>>(), _snapshotVersion), Times.Never);
        }
    }
}
