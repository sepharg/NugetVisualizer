using System;
using System.Collections.Generic;
using System.Text;
using TestStack.BDDfy;

namespace UnitTests
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;

    using Moq;
    using Moq.AutoMock;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Github;

    using Octokit;

    using Shouldly;

    using Xunit;

    public class GithubRepositoryReaderTests
    {
        private GithubRepositoryReader _githubRepositoryReader;

        private string _githubOrganization;

        private Mock<IGitHubClient> _githubclientMock;

        private string _repoName = "REPO NAME";

        private string _firstPath = "AtTheRoot.sln";

        private string _secondPath = "src/folder/infolder.sln";

        private string _thirdPath = "src/folder2/folder11/another.sln";

        private List<IProjectIdentifier> _projectIdentifiers;

        public GithubRepositoryReaderTests()
        {
            var autoMocker = new AutoMocker();
            _githubOrganization = "My org";
            _githubclientMock = autoMocker.GetMock<IGitHubClient>();
            autoMocker.GetMock<IGithubClientFactory>().Setup(x => x.GetClient(_githubOrganization, It.IsAny<ICredentialStore>())).Returns(_githubclientMock.Object);
            var configurationRootMock = autoMocker.GetMock<IConfigurationRoot>();
            configurationRootMock.Setup(x => x["GithubToken"]).Returns("TOKEN");
            configurationRootMock.Setup(x => x["GithubOrganization"]).Returns(_githubOrganization);
            autoMocker.GetMock<IConfigurationHelper>().Setup(x => x.GetConfiguration()).Returns(configurationRootMock.Object);
            _githubRepositoryReader = autoMocker.CreateInstance<GithubRepositoryReader>();
        }

        [Fact]

        public void GivenAGithubRepositoryWithSomeSolutions_WhenGetProjects_ThenNameAndPathAreParsedCorrectly()
        {
            this.Given(x => x.GivenAGithubRepositoryWithSomeSolutions())
                .When(x => x.WhenGetProjects())
                .Then(x => x.ThenNameAndPathAreParsedCorrectly())
                .BDDfy();
        }

        private void GivenAGithubRepositoryWithSomeSolutions()
        {
            var repository = new Repository(
                "url",
                "htmlUrl",
                "cloneUrl",
                "gitUrl",
                "sshUrl",
                "svnUrl",
                "mirrorUrl",
                1,
                null,
                _repoName,
                "fullName",
                "description",
                "homepage",
                "language",
                false,
                false,
                1,
                1,
                "defaultBranch",
                1,
                null,
                DateTimeOffset.Now,
                DateTimeOffset.Now,
                null,
                null,
                null,
                false,
                false,
                false,
                false,
                1,
                1,
                false,
                false,
                false);
            _githubclientMock.Setup(x => x.Repository.GetAllForOrg(_githubOrganization))
                .ReturnsAsync(
                    new List<Repository>()
                        {
                            repository
                        });
            
            _githubclientMock.Setup(x => x.Search.SearchCode(It.IsAny<SearchCodeRequest>()))
                .ReturnsAsync(
                    new SearchCodeResult(
                        3,
                        false,
                        new List<SearchCode>()
                            {
                                new SearchCode(
                                    "name",
                                    _firstPath,
                                    "sha",
                                    "url",
                                    "giturl",
                                    "htmlurl",
                                    repository),
                                new SearchCode(
                                    "name",
                                    _secondPath,
                                    "sha",
                                    "url",
                                    "giturl",
                                    "htmlurl",
                                    repository),
                                new SearchCode(
                                    "name",
                                    _thirdPath,
                                    "sha",
                                    "url",
                                    "giturl",
                                    "htmlurl",
                                    repository)
                            }));
        }

        private async Task WhenGetProjects()
        {
            _projectIdentifiers = await _githubRepositoryReader.GetProjectsAsync(_githubOrganization, new string[0]);
        }

        private void ThenNameAndPathAreParsedCorrectly()
        {
            _projectIdentifiers.Count.ShouldBe(3);
            _projectIdentifiers.First().SolutionName.ShouldBe("AtTheRoot");
            _projectIdentifiers.First().RepositoryName.ShouldBe(_repoName);
            _projectIdentifiers.First().Path.ShouldBe("");

            _projectIdentifiers.ElementAt(1).SolutionName.ShouldBe("infolder");
            _projectIdentifiers.ElementAt(1).RepositoryName.ShouldBe(_repoName);
            _projectIdentifiers.ElementAt(1).Path.ShouldBe("src/folder");

            _projectIdentifiers.ElementAt(2).SolutionName.ShouldBe("another");
            _projectIdentifiers.ElementAt(2).RepositoryName.ShouldBe(_repoName);
            _projectIdentifiers.ElementAt(2).Path.ShouldBe("src/folder2/folder11");
        }
    }
}