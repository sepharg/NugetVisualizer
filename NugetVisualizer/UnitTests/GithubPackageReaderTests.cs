namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Github;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class GithubPackageReaderTests
    {
        private GithubPackageReader _githubPackageReader;

        private ProjectIdentifier _projectIdentifier;

        private IEnumerable<XDocument> _packagesContents;

        public GithubPackageReaderTests()
        {
            _githubPackageReader = new GithubPackageReader();
        }

        [Fact]

        public void GivenAGithubProjectWithPackages_WhenReadingThePackagesForTheProject_ThenThePackagesFilesContentsAreReturned()
        {
            this.Given(x => x.GivenAGithubProjectWithPackages())
                .When(x => x.WhenReadingThePackagesForTheProject())
                .Then(x => x.ThenThePackagesFilesContentsAreReturned())
                .BDDfy();
        }

        private void GivenAGithubProjectWithPackages()
        {
            _projectIdentifier = new ProjectIdentifier("Moonpig.Migration.TemplateAssets", "");
        }

        private async Task WhenReadingThePackagesForTheProject()
        {
            _packagesContents = await _githubPackageReader.GetPackagesContentsAsync(_projectIdentifier);
        }

        private void ThenThePackagesFilesContentsAreReturned()
        {
            _packagesContents.Count().ShouldBe(5);
        }
    }
}
