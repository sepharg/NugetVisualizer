namespace UnitTests.IntegrationTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Autofac;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Github;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class GithubPackageReaderTests : IntegrationTest
    {
        private GithubPackageReader _githubPackageReader;

        private ProjectIdentifier _projectIdentifier;

        private IEnumerable<XDocument> _packagesContents;

        public GithubPackageReaderTests()
        {
            _githubPackageReader = ResolutionExtensions.Resolve<GithubPackageReader>(Container);
        }

        [Fact]

        public void GivenAGithubProjectWithPackages_WhenReadingThePackagesForTheProject_ThenThePackagesFilesContentsAreReturned()
        {
            BDDfyExtensions.BDDfy(
                    this.Given(x => x.GivenAGithubProjectWithPackages())
                        .When(x => x.WhenReadingThePackagesForTheProject())
                        .Then(x => x.ThenThePackagesFilesContentsAreReturned()));
        }

        private void GivenAGithubProjectWithPackages()
        {
            _projectIdentifier = new ProjectIdentifier("testrepo", "");
        }

        private async Task WhenReadingThePackagesForTheProject()
        {
            _packagesContents = await _githubPackageReader.GetPackagesContentsAsync(_projectIdentifier);
        }

        private void ThenThePackagesFilesContentsAreReturned()
        {
            Enumerable.Count<XDocument>(_packagesContents).ShouldBe(4);
        }
    }
}
