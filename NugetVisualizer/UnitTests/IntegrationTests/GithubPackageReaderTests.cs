namespace UnitTests.IntegrationTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Autofac;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Github;
    using NugetVisualizer.Core.PackageParser;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class GithubPackageReaderTests : IntegrationTest
    {
        private GithubPackageReader _githubPackageReader;

        private ProjectIdentifier _projectIdentifier;

        private IEnumerable<IPackageContainer> _packagesContents;

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
            _projectIdentifier = new ProjectIdentifier("testrepo", "testrepo", "");
        }

        private async Task WhenReadingThePackagesForTheProject()
        {
            _packagesContents = await _githubPackageReader.GetPackagesContentsAsync(_projectIdentifier);
        }

        private void ThenThePackagesFilesContentsAreReturned()
        {
            _packagesContents.Count().ShouldBe(5);
        }

        protected override void ExtraRegistrations(ContainerBuilder builder)
        {
        }
    }
}
