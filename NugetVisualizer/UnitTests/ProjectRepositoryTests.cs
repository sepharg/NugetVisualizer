namespace UnitTests
{
    using System.Collections.Generic;
    using System.Linq;

    using Autofac;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class ProjectRepositoryTests : IntegrationTest
    {
        private ProjectRepository _projectRepository;

        private List<Project> _projects;

        private PackageRepository _packageRepository;

        public ProjectRepositoryTests()
        {
            _projectRepository = Container.Resolve<ProjectRepository>();
            _packageRepository = new PackageRepository();
        }

        [Fact]

        public void GivenADatabaseWithSomeProjects_WhenLoadingProjects_ThenProjectsAreReturned()
        {
            this.Given(x => x.GivenADatabaseWithSomeProjects())
                .When(x => x.WhenLoadingProjects())
                .Then(x => x.ThenProjectsAreReturned())
                .BDDfy();
        }

        private void GivenADatabaseWithSomeProjects()
        {
            var projectsToCreate = GetProjectsToCreate();
            var packagesToCreate = GetPackagesToCreate();
            _packageRepository.AddRange(packagesToCreate);
            for (int i = 0; i < 10; i++)
            {
                _projectRepository.Add(projectsToCreate[i], packagesToCreate.Skip(i).Take(3).Select(p => p.Id));
            }
        }

        private void WhenLoadingProjects()
        {
            _projects = _projectRepository.LoadProjects();
        }

        private void ThenProjectsAreReturned()
        {
            _projects.Count.ShouldBe(10);
            _projects.First().ProjectPackages.Count.ShouldBe(3);
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
