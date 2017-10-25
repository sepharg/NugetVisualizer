namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using Moq;
    using Moq.AutoMock;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class ProjectRepositoryTests
    {
        private ProjectRepository _projectRepository;

        private Project _project;

        private AutoMocker _automocker;

        private Mock<INugetVisualizerContext> _databaseMock;

        public ProjectRepositoryTests()
        {
            _automocker = new AutoMocker();
            _databaseMock = _automocker.GetMock<DbContext>().As<INugetVisualizerContext>();
            _projectRepository = new ProjectRepository(_databaseMock.Object);
        }

        [Fact]

        public void GivenAnEmptyDatabase_WhenAProjectIsAddedForASnapshot_ThenTheProjectIsSavedOnce()
        {
            this.Given(x => x.GivenAnEmptyDatabase())
                .And(x => x.GivenAProject())
                .When(x => x.WhenSaveProjectForSnapshot(1))
                .Then(x => x.ThenTheProjectIsSavedOnce())
                .And(x => x.ThenProjectPackageIsLinkedForSnapshot(1))
                .BDDfy();
        }

        [Fact]

        public void GivenAProjectForASnapshotInTheDb_WhenSameProjectIsAddedForAnotherSnapshot_ThenTheProjectIsSavedOnceAndBothSnapshotRelationsArePersisted()
        {
            this.Given(x => x.GivenAProject())
                .And(x => x.GivenADatabaseWithAProject(1))
                .When(x => x.WhenSaveProjectForSnapshot(2))
                .Then(x => x.ThenTheProjectIsNotSaved())
                .And(x => x.ThenBothSnapshotRelationsArePersisted())
                .BDDfy();
        }

        private void GivenAProject()
        {
            _project = new Project("Proj");
        }

        private void WhenSaveProjectForSnapshot(int snapshotVersion)
        {
            _projectRepository.Add(_project, new List<int>() { 1, 2, 3 }, snapshotVersion);
        }

        private void ThenTheProjectIsSavedOnce()
        {
            _automocker.GetMock<DbSet<Project>>().Verify(x => x.Add(_project), Times.Once);
        }

        private void ThenTheProjectIsNotSaved()
        {
            _automocker.GetMock<DbSet<Project>>().Verify(x => x.Add(_project), Times.Never);
        }

        private void ThenProjectPackageIsLinkedForSnapshot(int snapshotVersion)
        {
            _project.ProjectPackages.Count.ShouldBe(3);
            _project.ProjectPackages.First().SnapshotVersion.ShouldBe(snapshotVersion);
        }

        private void ThenBothSnapshotRelationsArePersisted()
        {
            _project.ProjectPackages.Count.ShouldBe(6);
            _project.ProjectPackages.Count(x => x.SnapshotVersion == 1).ShouldBe(3);
            _project.ProjectPackages.Count(x => x.SnapshotVersion == 2).ShouldBe(3);
        }

        private void GivenAnEmptyDatabase()
        {
            var projectsInDb = new List<Project>().AsQueryable();
            var projectPackagesInDb = new List<ProjectPackage>().AsQueryable();

            SetupDbContents(projectsInDb, projectPackagesInDb);
        }

        private void SetDbsetMock<TType>(Mock<DbSet<TType>> dbsetMock, IQueryable<TType> typeInDb) where TType : class
        {
            dbsetMock.As<IQueryable<TType>>().Setup(m => m.Provider).Returns(typeInDb.Provider);
            dbsetMock.As<IQueryable<TType>>().Setup(m => m.Expression).Returns(typeInDb.Expression);
            dbsetMock.As<IQueryable<TType>>().Setup(m => m.ElementType).Returns(typeInDb.ElementType);
            dbsetMock.As<IQueryable<TType>>().Setup(m => m.GetEnumerator()).Returns(typeInDb.GetEnumerator());
        }

        private void SetupDbContents(IQueryable<Project> projectsInDb, IQueryable<ProjectPackage> projectPackagesInDb)
        {
            var projectSetMock = _automocker.GetMock<DbSet<Project>>();
            var projectPacakgesSetMock = _automocker.GetMock<DbSet<ProjectPackage>>();
            SetDbsetMock(projectSetMock, projectsInDb);
            SetDbsetMock(projectPacakgesSetMock, projectPackagesInDb);
            _databaseMock.Setup(x => x.Projects).Returns(projectSetMock.Object);
            _databaseMock.Setup(x => x.ProjectPackages).Returns(projectPacakgesSetMock.Object);
        }

        private void GivenADatabaseWithAProject(int snapshotVersion)
        {
            _project.ProjectPackages = new List<ProjectPackage>()
                                           {
                                               new ProjectPackage() { SnapshotVersion  = snapshotVersion, PackageId = 10 },
                                               new ProjectPackage() { SnapshotVersion  = snapshotVersion, PackageId = 11 },
                                               new ProjectPackage() { SnapshotVersion  = snapshotVersion, PackageId = 12 }
                                           };
            var projectsInDb = new List<Project>() { _project }.AsQueryable();
            var projectPackagesInDb = new List<ProjectPackage>() { new ProjectPackage() { Project = _project, SnapshotVersion = snapshotVersion } }.AsQueryable();

            SetupDbContents(projectsInDb, projectPackagesInDb);
        }
    }
}
