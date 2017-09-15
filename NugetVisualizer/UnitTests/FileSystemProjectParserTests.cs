namespace UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Autofac;

    using Microsoft.DotNet.PlatformAbstractions;

    using Moq.AutoMock;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.FileSystem;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class FileSystemProjectParserTests : DbTest
    {
        private FileSystemProjectParser _fileSystemProjectParser;

        private List<Project> _projects;

        private string _projecName;

        private string _projectPath;

        public FileSystemProjectParserTests()
        {
            _fileSystemProjectParser = Container.Resolve<FileSystemProjectParser>();
        }

        [Fact]

        public void GivenAProjectWithSomePackages_WhenParsingProject_ThenAProjectWithExpectedPackagesIsReturned()
        {
            this.Given(x => x.GivenAProjectWithSomePackages())
                .When(x => x.WhenParsingProject())
                .Then(x => x.ThenAProjectWithExpectedPackagesIsReturned())
                .BDDfy();
        }

        private void GivenAProjectWithSomePackages()
        {
            // folder21 has 6 packages, folder3 has 19 packages, root has 7 packages (total 32)
            _projecName = "Project1";
            _projectPath = ApplicationEnvironment.ApplicationBasePath + $"\\TestData\\{_projecName}";
        }

        private async Task WhenParsingProject()
        {
            _projects = await _fileSystemProjectParser.ParseProjectsAsync(new IProjectIdentifier[1] { new ProjectIdentifier(_projecName, _projectPath) } );
        }

        private void ThenAProjectWithExpectedPackagesIsReturned()
        {
            _projects.Single().Name.ShouldBe(_projecName);
            _projects.Single().ProjectPackages.Count.ShouldBe(29);
        }
    }
}
