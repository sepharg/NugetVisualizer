using TestStack.BDDfy;
namespace UnitTests
{
    using System;
    using System.Collections.Generic;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    using Shouldly;

    using Xunit;

    public class ProjectRepositoryTests
    {
        private ProjectRepository _projectRepository;

        private List<Project> _projects;

        public ProjectRepositoryTests()
        {
            _projectRepository = new ProjectRepository(new ConfigurationHelper());
            _projectRepository.DeleteProjects();
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
            _projectRepository.SaveProjects(projectsToCreate);
        }

        private void WhenLoadingProjects()
        {
            _projects = _projectRepository.LoadProjects();
        }

        private void ThenProjectsAreReturned()
        {
            _projects.Count.ShouldBe(10);
        }

        private List<Project> GetProjectsToCreate()
        {
            var projects = new List<Project>();

            for (int i = 0; i < 10; i++)
            {
                var project = new Project("Project " + i);
                for (int j = 0; j < 3; j++)
                {
                    project.ProjectPackages.Add(new ProjectPackage() { Package = new Package("Package " + j, "Version " + j, string.Empty), Project = project});
                }
                projects.Add(project);
            }

            return projects;
        }
    }
}
