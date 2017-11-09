namespace UnitTests.IntegrationTests.DbTests.InMemory
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Autofac;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class ProjectRepositoryTests : InMemoryDbTest
    {
        private const int _snapshotVersion = 1;

        private IProjectRepository _projectRepository;

        private List<Project> _projects;

        private IPackageRepository _packageRepository;
        
        public ProjectRepositoryTests()
        {
            _projectRepository = ResolutionExtensions.Resolve<IProjectRepository>(Container);
            _packageRepository = ResolutionExtensions.Resolve<IPackageRepository>(Container);
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

            _projectRepository.Add(projects[0], new List<int>() { package.Id }, _snapshotVersion);
            _projectRepository.Add(projects[1], new List<int>() { package.Id }, _snapshotVersion);
            _projectRepository.Add(projects[2], new List<int>() { package.Id }, _snapshotVersion);
            _projectRepository.Add(projects[3], new List<int>() { package.Id }, _snapshotVersion);
        }

        private void WhenLoadingProjects()
        {
            _projects = _projectRepository.LoadProjects(_snapshotVersion);
        }

        private async Task WhenGettingProjects(string package)
        {
            _projects = await _projectRepository.GetProjectsForPackage(package, 1);
        }

        private void ThenProjectsAreReturned()
        {
            ShouldBeTestExtensions.ShouldBe(_projects.Count, 10);
            Enumerable.First<Project>(_projects).ProjectPackages.Count.ShouldBe(3);
        }

        private void ThenProjectsForThePackageAreReturned()
        {
            ShouldBeTestExtensions.ShouldBe(_projects.Count, 4);
            Enumerable.First<Project>(_projects).ProjectPackages.Count.ShouldBe(1);
            Enumerable.First<Project>(_projects).ProjectPackages.Single().Package.ShouldNotBeNull();
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
                _projectRepository.Add(createdProjects[i], Enumerable.Skip<Package>(packagesToCreate, taken).Take(3).Select(p => p.Id), 1);
                taken += 3;
            }
        }
    }
}
