﻿namespace UnitTests.IntegrationTests.DbTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Autofac;

    using Microsoft.DotNet.PlatformAbstractions;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Dto;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    [Collection("DbIntegrationTests")]
    public class FileSystemProjectParserTests : IClassFixture<DbTest>
    {
        private readonly DbTest _dbTest;

        private IProjectParser _fileSystemProjectParser;

        private int _snapshotVersion = 2;

        private List<ParsedProject> _projects;

        private string _projecName;

        private string _projectPath;

        public FileSystemProjectParserTests(DbTest dbTest)
        {
            _dbTest = dbTest;
            _fileSystemProjectParser = ResolutionExtensions.Resolve<IProjectParser>(_dbTest.Container, new TypedParameter(typeof(ProjectParserType), ProjectParserType.FileSystem));
        }

        [Fact]

        public async Task GivenAProjectWithSomePackages_WhenParsingProject_ThenAProjectWithExpectedPackagesIsReturned()
        {
            BDDfyExtensions.BDDfy(
                    this.Given(x => x.GivenAProjectWithNetFrameworkOnlyPackages())
                        .When(x => x.WhenParsingProject())
                        .Then(x => x.ThenAProjectWithExpectedPackagesIsReturned(29)));
        }

        [Fact]

        public async Task GivenAProjectWithNetCoreAndNetFrameworkPackages_WhenParsingProject_ThenAProjectWithExpectedPackagesIsReturned()
        {
            BDDfyExtensions.BDDfy(
                this.Given(x => x.GivenAProjectWithNetCoreAndNetFrameworkPackages())
                    .When(x => x.WhenParsingProject())
                    .Then(x => x.ThenAProjectWithExpectedPackagesIsReturned(32)));
        }

        private void GivenAProjectWithNetFrameworkOnlyPackages()
        {
            // folder21 has 6 packages, folder3 has 19 packages, root has 7 packages (total 29 because Microsoft.Owin, Microsoft.Owin.Host.HttpListener and Microsoft.Owin.Hosting are repeated)
            //
            _projecName = "Project1";
            _projectPath = Path.Combine(ApplicationEnvironment.ApplicationBasePath, "TestData", _projecName);
        }

        private void GivenAProjectWithNetCoreAndNetFrameworkPackages()
        {
            // folder21 has 6 netframework packages, folder3 has 19 netframework packages, root has 7 netframework packages, folder1 has 3 net core packages (total 32 because Microsoft.Owin, Microsoft.Owin.Host.HttpListener and Microsoft.Owin.Hosting are repeated)
            _projecName = "Project2";
            _projectPath = Path.Combine(ApplicationEnvironment.ApplicationBasePath, "TestData", _projecName);
        }

        private async Task WhenParsingProject()
        {
            _projects = (await _fileSystemProjectParser.ParseProjectsAsync(new IProjectIdentifier[1] { new ProjectIdentifier(_projecName, "", _projectPath) }, _snapshotVersion)).ParsedProjects;
        }

        private void ThenAProjectWithExpectedPackagesIsReturned(int expected)
        {
            ShouldBeStringTestExtensions.ShouldBe(Enumerable.Single<ParsedProject>(_projects).ProjectName, _projecName);
            Enumerable.Single(_projects).ProjectPackageCount.ShouldBe(expected);
        }
    }
}
