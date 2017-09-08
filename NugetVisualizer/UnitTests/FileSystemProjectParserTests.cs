namespace UnitTests
{
    using Microsoft.DotNet.PlatformAbstractions;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.FileSystem;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class FileSystemProjectParserTests
    {
        private FileSystemProjectParser _fileSystemProjectParser;

        private Project _project;

        private string _projecName;

        private string _projectPath;

        public FileSystemProjectParserTests()
        {
            _fileSystemProjectParser = new FileSystemProjectParser();
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

        private void WhenParsingProject()
        {
            _project = _fileSystemProjectParser.ParseProject(new ProjectIdentifier(_projecName, _projectPath));
        }

        private void ThenAProjectWithExpectedPackagesIsReturned()
        {
            _project.Name.ShouldBe(_projecName);
            _project.Packages.Count.ShouldBe(32);
        }
    }
}
