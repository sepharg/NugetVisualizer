namespace UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Autofac;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Github;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class GithubProjectParserTests : IntegrationTest
    {
        private GithubProjectParser _githubProjectParser;
        
        private IEnumerable<XDocument> _packagesContents;

        private IEnumerable<Project> _projects;

        private List<IProjectIdentifier> _projectIdentifiers;

        public GithubProjectParserTests()
        {
            _githubProjectParser = Container.Resolve<GithubProjectParser>();
        }

        [Fact]

        public void GivenAGithubOrganisationWithProjectsAndPackages_WhenReadingThePackagesForTheProjects_ThenThePackagesFilesContentsAreReturned()
        {
            this.Given(x => x.GivenAGithubOrganisationWithProjectsAndPackages())
                .When(x => x.WhenReadingThePackagesForTheProjects())
                .Then(x => x.ThenThePackagesFilesContentsAreReturned())
                .BDDfy();
        }

        private async Task GivenAGithubOrganisationWithProjectsAndPackages()
        {
            var githubRepositoryReader = new GithubRepositoryReader(new ConfigurationHelper());
            _projectIdentifiers = await githubRepositoryReader.GetProjectsAsync("photobox", new []{ "moonpig", "template" });
        }

        private async Task WhenReadingThePackagesForTheProjects()
        {
            
            _projects = await _githubProjectParser.ParseProjectsAsync(_projectIdentifiers);
        }

        private void ThenThePackagesFilesContentsAreReturned()
        {
            _projects.ShouldNotBeNull();
            _projects.Count().ShouldBeGreaterThan(0);
        }
    }
}
