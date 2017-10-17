namespace UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Moq;
    using Moq.AutoMock;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class ProcessorTests
    {
        private AutoMocker _autoMocker;

        private Processor _processor;

        private int _snapshotVersion = 3;
        
        private List<IProjectIdentifier> _projectIdentifiers;

        private IEnumerable<IProjectIdentifier> _parsedProjects;

        public ProcessorTests()
        {
            _autoMocker = new AutoMocker();
            _processor = _autoMocker.CreateInstance<Processor>();
            _projectIdentifiers = new List<IProjectIdentifier>()
                                      {
                                          new ProjectIdentifier("first", "path"),
                                          new ProjectIdentifier("second", "path2"),
                                          new ProjectIdentifier("third", "path3"),
                                          new ProjectIdentifier("fourth", "path4"),
                                      };
            _autoMocker.GetMock<IProjectParser>()
                .Setup(x => x.ParseProjectsAsync(It.IsAny<IEnumerable<IProjectIdentifier>>(), _snapshotVersion))
                .ReturnsAsync(() => new ProjectParsingResult(null, false))
                .Callback<IEnumerable<IProjectIdentifier>, int>((identifiers, snapshotVersion) => _parsedProjects = identifiers);
        }

        [Fact]

        public void GivenNoProcessResumeNeeded_WhenProcess_ThenAllItemsAreProcessed()
        {
            this.Given(x => x.GivenNoProcessResumeNeeded())
                .And(x => x.GivenThereAreProjectsToProcess())
                .When(x => x.WhenProcess())
                .Then(x => x.ThenAllItemsAreProcessed())
                .BDDfy();
        }

        [Fact]

        public void GivenProcessResumeNeeded_WhenProcess_ThenOnlyRemainingItemsAreProcessed()
        {
            this.Given(x => x.GivenProcessResumeNeeded())
                .And(x => x.GivenThereAreProjectsToProcess())
                .When(x => x.WhenProcess())
                .Then(x => x.ThenOnlyRemainingItemsAreProcessed())
                .BDDfy();
        }

        private void GivenNoProcessResumeNeeded()
        {
            _autoMocker.GetMock<IProjectParsingState>().Setup(x => x.GetLatestParsedProject()).Returns<string>(null);
        }

        private void GivenProcessResumeNeeded()
        {
            _autoMocker.GetMock<IProjectParsingState>().Setup(x => x.GetLatestParsedProject()).Returns("second");
        }

        private void GivenThereAreProjectsToProcess()
        {
            _autoMocker.GetMock<IRepositoryReader>()
                .Setup(x => x.GetProjectsAsync(string.Empty, null))
                .ReturnsAsync(_projectIdentifiers);
        }

        private async Task WhenProcess()
        {
            await _processor.Process(string.Empty, null, _snapshotVersion);
        }

        private void ThenAllItemsAreProcessed()
        {
            _parsedProjects.ShouldBeSameAs(_projectIdentifiers);
            _autoMocker.GetMock<IProjectParser>().Verify(x => x.ParseProjectsAsync(It.IsAny<IEnumerable<IProjectIdentifier>>(), _snapshotVersion));
        }

        private void ThenOnlyRemainingItemsAreProcessed()
        {
            _parsedProjects.ShouldBe(_projectIdentifiers.Skip(2));
        }
    }
}
