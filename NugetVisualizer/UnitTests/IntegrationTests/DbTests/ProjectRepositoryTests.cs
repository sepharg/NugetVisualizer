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
            InitializeBasicData();
        }

        [Fact]

        public void GivenADatabaseWithSomeProjects_WhenLoadingProjects_ThenProjectsAreReturned()
        {
            BDDfyExtensions.BDDfy(
                    this.When(x => x.WhenLoadingProjects())
                        .Then(x => x.ThenProjectsAreReturned()));
        }

        [Fact]

        public void GivenADatabaseWithSomeProjects_WhenGettingProjectsForPackage_ThenProjectsAreReturned()
        {
            BDDfyExtensions.BDDfy(
                this.Given(x => x.GivenAPackageInProjects())
                    .When(x => x.WhenGettingProjects("Great Package"))
                    .Then(x => x.ThenProjectsForThePackageAreReturned()));
        }

        private void GivenAPackageInProjects()
        {
            var package = new Package("Great Package", "1111.111", string.Empty);

            _packageRepository.Add(package);
            var projects = new List<Project>()
                               {
                                   new Project("P1"),
                                   new Project("P2"),
                                   new Project("P3"),
                                   new Project("P4")
                               };

            _projectRepository.Add(projects[0], new List<int>() { package.Id });
            _projectRepository.Add(projects[1], new List<int>() { package.Id });
            _projectRepository.Add(projects[2], new List<int>() { package.Id });
            _projectRepository.Add(projects[3], new List<int>() { package.Id });
        }

        private void WhenLoadingProjects()
        {
            _projects = _projectRepository.LoadProjects();
        }

        private void WhenGettingProjects(string package)
        {
            _projects = _projectRepository.GetProjectsForPackage(package);
        }

        private void ThenProjectsAreReturned()
        {
            ShouldBeTestExtensions.ShouldBe(_projects.Count, 10);
            Enumerable.First<Project>(_projects).ProjectPackages.Count.ShouldBe(3);
        }

        private void ThenProjectsForThePackageAreReturned()
        {
            ShouldBeTestExtensions.ShouldBe(_projects.Count, 4);
            _projects.First().ProjectPackages.Count.ShouldBe(1);
            _projects.First().ProjectPackages.Single().Package.ShouldNotBeNull();
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

        private List<Project> GetProjectsToCreate(int numberOfProjects)
        {
            var projects = new List<Project>();
            for (int i = 0; i < numberOfProjects; i++)
            {
                var project = new Project("Project " + i);
                projects.Add(project);
            }
            return projects;
        }

        private void InitializeBasicData()
        {
            var createdProjects = GetProjectsToCreate(10);
            var packagesToCreate = GetPackagesToCreate();
            _packageRepository.AddRange(packagesToCreate);
            int taken = 0;
            // proj0 - package0, package1, package2, proj1 - package3, package4, package5, ....
            for (int i = 0; i < 10; i++)
            {
                _projectRepository.Add(createdProjects[i], packagesToCreate.Skip(taken).Take(3).Select(p => p.Id));
                taken += 3;
            }
        }
    }
}
