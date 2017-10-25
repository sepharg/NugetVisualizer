namespace UnitTests
{
    using System;
    using System.IO;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Repositories;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class ProjectParsingStateTests : IDisposable
    {
        private IProjectParsingState _projectParsingState;

        private string _projectName;

        private string _latestParsedProject;

        public ProjectParsingStateTests()
        {
            _projectParsingState = new ProjectParsingStateRepository();
        }

        [Fact]

        public void GivenProjectParsingStateFileDoesntExist_WhenSavingProject_ThenProjectSavingFileCreated()
        {
            this.Given(x => x.GivenProjectParsingStateFileDoesntExist())
                .And(x => x.GivenAProjectToSave())
                .When(x => x.WhenSavingProject())
                .Then(x => x.ThenProjectSavingFileCreated())
                .BDDfy();
        }

        [Fact]

        public void GivenProjectParsingStateFileDoesntExist_WhenGettingProject_ThenNothingIsReturned()
        {
            this.Given(x => x.GivenProjectParsingStateFileDoesntExist())
                .When(x => x.WhenGettingProject())
                .Then(x => x.ThenNothingIsReturned())
                .BDDfy();
        }

        [Fact]

        public void GivenProjectParsingStateFileExists_WhenGettingProject_ThenProjectIsReturned()
        {
            this.Given(x => x.GivenAProjectToSave())
                .And(x => x.WhenSavingProject())
                .When(x => x.WhenGettingProject())
                .Then(x => x.ThenProjectIsReturned())
                .BDDfy();
        }

        [Fact]

        public void GivenProjectParsingStateFileExists_WhenDeletingProjectState_ThenNothingIsReturned()
        {
            this.Given(x => x.GivenAProjectToSave())
                .And(x => x.WhenSavingProject())
                .And(x => x.WhenDeletingProject())
                .When(x => x.WhenGettingProject())
                .Then(x => x.ThenNothingIsReturned())
                .BDDfy();
        }

        [Fact]

        public void WhenSavingProject_ThenProjectIsSavedInFile()
        {
            this.Given(x => x.GivenAProjectToSave())
                .When(x => x.WhenSavingProject())
                .Then(x => x.ThenProjectIsSaved())
                .BDDfy();
        }

        private void GivenProjectParsingStateFileDoesntExist()
        {
            if (File.Exists(ProjectParsingStateRepository.ProjectParsingStateFilename))
            {
                File.Delete(ProjectParsingStateRepository.ProjectParsingStateFilename);
            }
            Assert.False(File.Exists(ProjectParsingStateRepository.ProjectParsingStateFilename));
        }

        private void GivenAProjectToSave()
        {
            _projectName = "project1";
        }

        private void WhenSavingProject()
        {
            _projectParsingState.SaveLatestParsedProject(_projectName);
        }

        private void WhenDeletingProject()
        {
            _projectParsingState.DeleteLatestParsedProject();
        }

        private void WhenGettingProject()
        {
            _latestParsedProject = _projectParsingState.GetLatestParsedProject();
        }

        private void ThenProjectSavingFileCreated()
        {
            File.Exists(ProjectParsingStateRepository.ProjectParsingStateFilename);
        }

        private void ThenProjectIsSaved()
        {
            var fileText = File.ReadAllText(ProjectParsingStateRepository.ProjectParsingStateFilename);
            fileText.ShouldBe(_projectName);
        }

        private void ThenProjectIsReturned()
        {
            _latestParsedProject.ShouldBe(_projectName);
        }

        private void ThenNothingIsReturned()
        {
            _latestParsedProject.ShouldBeNull();
        }

        public void Dispose()
        {
            if (File.Exists(ProjectParsingStateRepository.ProjectParsingStateFilename))
            {
                File.Delete(ProjectParsingStateRepository.ProjectParsingStateFilename);
            }
        }
    }
}
