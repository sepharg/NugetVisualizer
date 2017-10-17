namespace UnitTests.IntegrationTests.DbTests.InMemory
{
    using System.Collections.Generic;
    using System.Linq;

    using Autofac;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

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
            BDDfyExtensions.BDDfy(
                    this.Given(x => x.GivenAnInitialState())
                        .When(x => x.WhenGetPackagesOrderedByVersionCount())
                        .Then(x => x.ThenCorrectOrderReturned()));
        }

        [Fact]

        public void GivenSomePackagesWithDifferentUsagesByProjects_WhenGetMostUsedPackages_ThenCorrectOrderReturnedForMostUsedPackages()
        {
            BDDfyExtensions.BDDfy(
                    this.Given(x => x.GivenSomePackagesWithDifferentUsagesByProjects())
                        .When(x => x.WhenGetMostUsedPackages())
                        .Then(x => x.ThenCorrectOrderReturnedForMostUsedPackages()));
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
            _projectRepository.Add(new Project("P1"), new[] { Enumerable.ElementAt<Package>(_packages, 0).Id, Enumerable.ElementAt<Package>(_packages, 1).Id, Enumerable.ElementAt<Package>(_packages, 3).Id, Enumerable.ElementAt<Package>(_packages, 4).Id, Enumerable.ElementAt<Package>(_packages, 6).Id, Enumerable.ElementAt<Package>(_packages, 11).Id }, 1);
            _projectRepository.Add(new Project("P2"), new[] { Enumerable.ElementAt<Package>(_packages, 0).Id, Enumerable.ElementAt<Package>(_packages, 1).Id, Enumerable.ElementAt<Package>(_packages, 2).Id, Enumerable.ElementAt<Package>(_packages, 10).Id }, 1);
            _projectRepository.Add(new Project("P3"), new[] { Enumerable.ElementAt<Package>(_packages, 1).Id, Enumerable.ElementAt<Package>(_packages, 2).Id, Enumerable.ElementAt<Package>(_packages, 5).Id, Enumerable.ElementAt<Package>(_packages, 7).Id, Enumerable.ElementAt<Package>(_packages, 8).Id, Enumerable.ElementAt<Package>(_packages, 9).Id, Enumerable.ElementAt<Package>(_packages, 10).Id, Enumerable.ElementAt<Package>(_packages, 13).Id }, 1);
            _projectRepository.Add(new Project("UsagesP4"), new[] { Enumerable.ElementAt<Package>(_packages, 0).Id, Enumerable.ElementAt<Package>(_packages, 10).Id }, 1);
            _projectRepository.Add(new Project("UsagesP5"), new[] { Enumerable.ElementAt<Package>(_packages, 11).Id, Enumerable.ElementAt<Package>(_packages, 12).Id }, 1);

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
            _projectRepository.Add(new Project("P1"), new []{ Enumerable.ElementAt<Package>(_packages, 0).Id, Enumerable.ElementAt<Package>(_packages, 1).Id, Enumerable.ElementAt<Package>(_packages, 3).Id, Enumerable.ElementAt<Package>(_packages, 4).Id }, 1);
            _projectRepository.Add(new Project("P2"), new []{ Enumerable.ElementAt<Package>(_packages, 1).Id, Enumerable.ElementAt<Package>(_packages, 2).Id, Enumerable.ElementAt<Package>(_packages, 5).Id }, 1);
            _projectRepository.Add(new Project("P3"), new []{ Enumerable.ElementAt<Package>(_packages, 0).Id }, 1);

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
            var filteredResults = Enumerable.Where<KeyValuePair<Package, int>>(_mostUsedPackages, x => new [] { "MostUsed", "SecondMostUsed", "ThirdMostUsed", "FourthMostUsed", "FifthMostUsed" }.Contains(x.Key.Name)).ToDictionary(x => x.Key, x => x.Value); // this is a workaround because the tests share the database. should be fixed as part of https://github.com/sepharg/NugetVisualizer/issues/5 (basically remove the filter)
            filteredResults.Keys.Count.ShouldBe(5);
            filteredResults[filteredResults.Keys.Single(p => p.Name.Equals("MostUsed"))].ShouldBe(9);
            filteredResults[filteredResults.Keys.Single(p => p.Name.Equals("SecondMostUsed"))].ShouldBe(6);
            filteredResults[filteredResults.Keys.Single(p => p.Name.Equals("ThirdMostUsed"))].ShouldBe(3);
            filteredResults[filteredResults.Keys.Single(p => p.Name.Equals("FourthMostUsed"))].ShouldBe(3);
            filteredResults[filteredResults.Keys.Single(p => p.Name.Equals("FifthMostUsed"))].ShouldBe(1);
        }

        private void ThenCorrectOrderReturned()
        {
            ShouldBeTestExtensions.ShouldBe(_packagesOrderedByVersionsCount.Keys.Count, 3);
            ShouldBeTestExtensions.ShouldBe(_packagesOrderedByVersionsCount[Enumerable.Single<Package>(_packagesOrderedByVersionsCount.Keys, p => p.Name.Equals("Package1"))], 3); // 3 versions, 1.0, 1.1 & 1.2
            ShouldBeTestExtensions.ShouldBe(_packagesOrderedByVersionsCount[Enumerable.Single<Package>(_packagesOrderedByVersionsCount.Keys, p => p.Name.Equals("Package2"))], 1); // 1 version 2.6
            ShouldBeTestExtensions.ShouldBe(_packagesOrderedByVersionsCount[Enumerable.Single<Package>(_packagesOrderedByVersionsCount.Keys, p => p.Name.Equals("Package3"))], 2); // 2 versions, 5.0 & 5.1
        }
    }
}
