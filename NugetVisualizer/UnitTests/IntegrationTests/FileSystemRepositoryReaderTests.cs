namespace UnitTests.IntegrationTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Autofac;

    using Microsoft.DotNet.PlatformAbstractions;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.FileSystem;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class FileSystemRepositoryReaderTests : IntegrationTest
    {
        private string _rootPath;

        private FileSystemRepositoryReader _fileSystemRepositoryReader;

        private List<IProjectIdentifier> _projectIdentifiers;

        public FileSystemRepositoryReaderTests()
        {
            _fileSystemRepositoryReader = ResolutionExtensions.Resolve<FileSystemRepositoryReader>(Container);
        }

        [Fact]

        public void GivenSomeFoldersWithSolutionFiles_WhenGetProjects_ThenAProjectForEachSolutionFileWithNugetIsReturned()
        {
            this.Given(x => x.GivenSomeFoldersWithSolutionFiles())
                .When(x => x.WhenGetProjects())
                .Then(x => x.ThenAProjectForEachSolutionFileWithNugetIsReturned())
                .BDDfy();
        }

        private void GivenSomeFoldersWithSolutionFiles()
        {
            _rootPath = Path.Combine(ApplicationEnvironment.ApplicationBasePath, "TestData");

        }

        private async Task WhenGetProjects()
        {
            _projectIdentifiers = await _fileSystemRepositoryReader.GetProjectsAsync(_rootPath, new string[0]);
        }

        private void ThenAProjectForEachSolutionFileWithNugetIsReturned()
        {
            _projectIdentifiers.Count.ShouldBe(3);
            var oneProject = _projectIdentifiers.SingleOrDefault(x => x.Name.Equals("MySol1"));
            oneProject.ShouldNotBeNull();
            oneProject.Path.ShouldEndWith($"TestData{Path.DirectorySeparatorChar}FolderWithTwoSolutions{Path.DirectorySeparatorChar}Sol1");
            _projectIdentifiers.SingleOrDefault(x => x.Name.Equals("Sol2")).ShouldNotBeNull();
            _projectIdentifiers.SingleOrDefault(x => x.Name.Equals("NoPackages")).ShouldNotBeNull();
        }

        protected override void ExtraRegistrations(ContainerBuilder builder)
        {
        }
    }
}
