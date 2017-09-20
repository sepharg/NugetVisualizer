namespace UnitTests.IntegrationTests.DbTests
{
    using System.Collections.Generic;
    using System.Linq;

    using Autofac;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    [Collection("DbIntegrationTests")]
    public class ProjectRepositoryTests : IClassFixture<DbTest>
    {
        private readonly DbTest _dbTest;

        private IProjectRepository _projectRepository;

        private List<Project> _projects;

        private IPackageRepository _packageRepository;

        public ProjectRepositoryTests(DbTest dbTest)
        {
            _dbTest = dbTest;
            _projectRepository = ResolutionExtensions.Resolve<IProjectRepository>(_dbTest.Container);
            _packageRepository = ResolutionExtensions.Resolve<IPackageRepository>(_dbTest.Container);
        }

        [Fact]

        public void GivenADatabaseWithSomeProjects_WhenLoadingProjects_ThenProjectsAreReturned()
        {
            BDDfyExtensions.BDDfy(
                    this.Given(x => x.GivenADatabaseWithSomeProjects())
                        .When(x => x.WhenLoadingProjects())
                        .Then(x => x.ThenProjectsAreReturned()));
        }

        private void GivenADatabaseWithSomeProjects()
        {
            var projectsToCreate = GetProjectsToCreate();
            var packagesToCreate = GetPackagesToCreate();
            _packageRepository.AddRange(packagesToCreate);
            int taken = 0;
            for (int i = 0; i < 10; i++)
            {
                _projectRepository.Add(projectsToCreate[i], Enumerable.Skip<Package>(packagesToCreate, taken).Take(3).Select(p => p.Id));
                taken += 3;
            }
        }

        private void WhenLoadingProjects()
        {
            _projects = _projectRepository.LoadProjects();
        }

        private void ThenProjectsAreReturned()
        {
            ShouldBeTestExtensions.ShouldBe(_projects.Count, 10);
            Enumerable.First<Project>(_projects).ProjectPackages.Count.ShouldBe(3);
        }

        private List<Package> GetPackagesToCreate()
        {
            var packages = new List<Package>();
            for (int j = 0; j < 30; j++)
            {
                packages.Add(new Package("Package " + j, "Version " + j, string.Empty));
            }
            return packages;
        }

        private List<Project> GetProjectsToCreate()
        {
            var projects = new List<Project>();
            for (int i = 0; i < 10; i++)
            {
                var project = new Project("Project " + i);
                projects.Add(project);
            }
            return projects;
        }
    }
}
