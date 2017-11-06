namespace UnitTests.IntegrationTests.DbTests
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

    [Collection("DbIntegrationTests")]
    public class GithubProjectParserTests : IClassFixture<DbTest>
    {
        private readonly DbTest _dbTest;

        private IProjectParser _githubProjectParser;

        private int _snapshotVersion = 1;

        private IEnumerable<Project> _projects;

        private List<IProjectIdentifier> _projectIdentifiers;

        public GithubProjectParserTests(DbTest dbTest)
        {
            _dbTest = dbTest;
            _githubProjectParser = ResolutionExtensions.Resolve<IProjectParser>(_dbTest.Container, new TypedParameter(typeof(ProjectParserType), ProjectParserType.Github));
        }

        [Fact]

        public void GivenAGithubOrganisationWithProjectsAndPackages_WhenReadingThePackagesForTheProjects_ThenThePackagesFilesContentsAreReturned()
        {
            BDDfyExtensions.BDDfy(
                    this.Given(x => x.GivenAGithubOrganisationWithProjectsAndPackages())
                        .When(x => x.WhenReadingThePackagesForTheProjects())
                        .Then(x => x.ThenThePackagesFilesContentsAreReturned()));
        }

        private async Task GivenAGithubOrganisationWithProjectsAndPackages()
        {
            var githubRepositoryReader = new GithubRepositoryReader(new ConfigurationHelper(), new GithubClientFactory());
            _projectIdentifiers = await githubRepositoryReader.GetProjectsAsync("sephargorganization", new[] { "testrepo" });
        }

        private async Task WhenReadingThePackagesForTheProjects()
        {
            
            _projects = (await _githubProjectParser.ParseProjectsAsync(_projectIdentifiers, _snapshotVersion)).ParsedProjects;
        }

        private void ThenThePackagesFilesContentsAreReturned()
        {
            ShouldBeNullExtensions.ShouldNotBeNull<IEnumerable<Project>>(_projects);
            Enumerable.Count<Project>(_projects).ShouldBeGreaterThan(0);
        }
    }
}
