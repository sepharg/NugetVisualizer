namespace UnitTests.IntegrationTests.DbTests
{
    using System.Collections.Generic;
    using System.Linq;

    using Autofac;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    using Shouldly;

    using Xunit;
    using TestStack.BDDfy;


    [Collection("DbIntegrationTests")]
    public class PackageRepositoryTests : IClassFixture<DbTest>
    {
        private readonly DbTest _dbTest;

        private IProjectRepository _projectRepository;

        private IPackageRepository _packageRepository;

        private Dictionary<Package, int> _packagesOrderedByVersionsCount;

        private List<Package> _packages;

        public PackageRepositoryTests(DbTest dbTest)
        {
            _dbTest = dbTest;
            _projectRepository = ResolutionExtensions.Resolve<IProjectRepository>(_dbTest.Container);
            _packageRepository = ResolutionExtensions.Resolve<IPackageRepository>(_dbTest.Container);
        }


        [Fact]

        public void GivenAnInitialState_WhenGetPackagesOrderedByVersionCount_ThenCorrectOrderReturned()
        {
            this.Given(x => x.GivenAnInitialState())
                .When(x => x.WhenGetPackagesOrderedByVersionCount())
                .Then(x => x.ThenCorrectOrderReturned())
                .BDDfy();
        }

        private void GivenAnInitialState()
        {
            /* Package1 
             * 
             * P1    1.0   1.1
             * P2          1.1 1.2
             * P3    1.0
             * 
             * Package2
             * 
             * P1    2.6
             * 
             * 
             * Package3
             * 
             * P1   5.0
             * P2         5.1
             * 
             */

            _packages = new List<Package>()
                            {
                                new Package("Package1", "1.0", string.Empty),
                                new Package("Package1", "1.1", string.Empty),
                                new Package("Package1", "1.2", string.Empty),
                                new Package("Package2", "2.6", string.Empty),
                                new Package("Package3", "5.0", string.Empty),
                                new Package("Package3", "5.1", string.Empty)
                            };
            _packageRepository.AddRange(_packages);
            _projectRepository.Add(new Project("P1"), new []{ _packages.ElementAt(0).Id, _packages.ElementAt(1).Id, _packages.ElementAt(3).Id, _packages.ElementAt(4).Id } );
            _projectRepository.Add(new Project("P2"), new []{ _packages.ElementAt(1).Id, _packages.ElementAt(2).Id, _packages.ElementAt(5).Id } );
            _projectRepository.Add(new Project("P3"), new []{ _packages.ElementAt(0).Id } );

        }

        private void WhenGetPackagesOrderedByVersionCount()
        {
            _packagesOrderedByVersionsCount = _packageRepository.GetPackagesOrderedByVersionsCount();
        }

        private void ThenCorrectOrderReturned()
        {
            _packagesOrderedByVersionsCount.Keys.Count.ShouldBe(3);
            _packagesOrderedByVersionsCount[_packagesOrderedByVersionsCount.Keys.Single(p => p.Name.Equals("Package1"))].ShouldBe(3); // 3 versions, 1.0, 1.1 & 1.2
            _packagesOrderedByVersionsCount[_packagesOrderedByVersionsCount.Keys.Single(p => p.Name.Equals("Package2"))].ShouldBe(1); // 1 version 2.6
            _packagesOrderedByVersionsCount[_packagesOrderedByVersionsCount.Keys.Single(p => p.Name.Equals("Package3"))].ShouldBe(2); // 2 versions, 5.0 & 5.1
        }
    }
}
