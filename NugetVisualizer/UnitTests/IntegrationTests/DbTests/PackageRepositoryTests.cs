namespace UnitTests.IntegrationTests.DbTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Autofac;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    using Shouldly;

    using Xunit;
    using TestStack.BDDfy;

    using Xunit.Abstractions;

    public class PackageRepositoryTests : InMemoryDbTest
    {
        private IProjectRepository _projectRepository;

        private IPackageRepository _packageRepository;

        private Dictionary<Package, int> _packagesOrderedByVersionsCount;

        private Dictionary<Package, int> _mostUsedPackages;

        private List<Package> _packages;

        public PackageRepositoryTests()
        {
            _projectRepository = ResolutionExtensions.Resolve<IProjectRepository>(Container);
            _packageRepository = ResolutionExtensions.Resolve<IPackageRepository>(Container);
        }

        [Fact]

        public void GivenAnInitialState_WhenGetPackagesOrderedByVersionCount_ThenCorrectOrderReturned()
        {
            this.Given(x => x.GivenAnInitialState())
                .When(x => x.WhenGetPackagesOrderedByVersionCount())
                .Then(x => x.ThenCorrectOrderReturned())
                .BDDfy();
        }

        [Fact]

        public void GivenSomePackagesWithDifferentUsagesByProjects_WhenGetMostUsedPackages_ThenCorrectOrderReturnedForMostUsedPackages()
        {
            this.Given(x => x.GivenSomePackagesWithDifferentUsagesByProjects())
                .When(x => x.WhenGetMostUsedPackages())
                .Then(x => x.ThenCorrectOrderReturnedForMostUsedPackages())
                .BDDfy();
        }

        private void GivenSomePackagesWithDifferentUsagesByProjects()
        {
            /*
             * MostUsed : 9 usages. UsagesP1 : 3 (1.0, 1.1, 1.3). UsagesP2 : 5 (1.0, 1.1, 1.2). UsagesP3 : 2 (1.1, 1.2). UsagesP4 : 1 (1.0)
             * SecondMostUsed : 6 usages. UsagesP1 : 2 (1.0, 1.2) . UsagesP3 : 4 (1.1, 1.3, 1.4, 1.5)
             * ThirdMostUsed : 3 usages. UsagesP2 : 1 (4.3). UsagesP3 : 1 (4.3). UsagesP4 : 1 (4.3)
             * FourthMostUsed : 3 usages. UsagesP1: 1 (2.0) . UsagesP5 : 2 (2.0, 2.11)
             * FifthMostUsed : 1 usage . UsagesP3 : 1 (5.0)
             */

            _packages = new List<Package>()
                            {
                                new Package("MostUsed", "1.0", string.Empty),
                                new Package("MostUsed", "1.1", string.Empty),
                                new Package("MostUsed", "1.2", string.Empty),
                                new Package("MostUsed", "1.3", string.Empty),
                                new Package("SecondMostUsed", "1.0", string.Empty),
                                new Package("SecondMostUsed", "1.1", string.Empty),
                                new Package("SecondMostUsed", "1.2", string.Empty), 
                                new Package("SecondMostUsed", "1.3", string.Empty), 
                                new Package("SecondMostUsed", "1.4", string.Empty), 
                                new Package("SecondMostUsed", "1.5", string.Empty), 
                                new Package("ThirdMostUsed", "4.3", string.Empty), 
                                new Package("FourthMostUsed", "2.0", string.Empty), 
                                new Package("FourthMostUsed", "2.11", string.Empty),
                                new Package("FifthMostUsed", "5.0", string.Empty)
                            };
            _packageRepository.AddRange(_packages);
            _projectRepository.Add(new Project("P1"), new[] { _packages.ElementAt(0).Id, _packages.ElementAt(1).Id, _packages.ElementAt(3).Id, _packages.ElementAt(4).Id, _packages.ElementAt(6).Id, _packages.ElementAt(11).Id });
            _projectRepository.Add(new Project("P2"), new[] { _packages.ElementAt(0).Id, _packages.ElementAt(1).Id, _packages.ElementAt(2).Id, _packages.ElementAt(10).Id });
            _projectRepository.Add(new Project("P3"), new[] { _packages.ElementAt(1).Id, _packages.ElementAt(2).Id, _packages.ElementAt(5).Id, _packages.ElementAt(7).Id, _packages.ElementAt(8).Id, _packages.ElementAt(9).Id, _packages.ElementAt(10).Id, _packages.ElementAt(13).Id });
            _projectRepository.Add(new Project("UsagesP4"), new[] { _packages.ElementAt(0).Id, _packages.ElementAt(10).Id });
            _projectRepository.Add(new Project("UsagesP5"), new[] { _packages.ElementAt(11).Id, _packages.ElementAt(12).Id });

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

        private void WhenGetMostUsedPackages()
        {
            _mostUsedPackages = _packageRepository.GetPackageUses();
        }

        private void ThenCorrectOrderReturnedForMostUsedPackages()
        {
            var filteredResults = _mostUsedPackages.Where(x => new [] { "MostUsed", "SecondMostUsed", "ThirdMostUsed", "FourthMostUsed", "FifthMostUsed" }.Contains(x.Key.Name)).ToDictionary(x => x.Key, x => x.Value); // this is a workaround because the tests share the database. should be fixed as part of https://github.com/sepharg/NugetVisualizer/issues/5 (basically remove the filter)
            filteredResults.Keys.Count.ShouldBe(5);
            filteredResults[filteredResults.Keys.Single(p => p.Name.Equals("MostUsed"))].ShouldBe(9);
            filteredResults[filteredResults.Keys.Single(p => p.Name.Equals("SecondMostUsed"))].ShouldBe(6);
            filteredResults[filteredResults.Keys.Single(p => p.Name.Equals("ThirdMostUsed"))].ShouldBe(3);
            filteredResults[filteredResults.Keys.Single(p => p.Name.Equals("FourthMostUsed"))].ShouldBe(3);
            filteredResults[filteredResults.Keys.Single(p => p.Name.Equals("FifthMostUsed"))].ShouldBe(1);
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
