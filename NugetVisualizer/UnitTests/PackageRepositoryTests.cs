namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Moq;
    using Moq.AutoMock;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    using Shouldly;

    using TestStack.BDDfy;

    using Xunit;

    public class PackageRepositoryTests
    {
        private PackageRepository _packageRepository;

        private AutoMocker _automocker;

        private Mock<INugetVisualizerContext> _databaseMock;

        private Dictionary<Package, int> _packageUses;

        private Package _package1;

        private Package _package2;

        private Package _package3;

        public PackageRepositoryTests()
        {
            _automocker = new AutoMocker();
            _databaseMock = _automocker.GetMock<DbContext>().As<INugetVisualizerContext>();
            _packageRepository = new PackageRepository(_databaseMock.Object);
        }

        //[Theory] -- Had to comment it out because now it's usin raw sql instead of accessing the collections. probably delete because it's covered by db integration test.
        [InlineData(1,1,1,0)]
        [InlineData(2,0,0,1)]

        public void GivenADatabaseWithTwoSnapshots_WhenGetPackageUses_ThenOnlyUsesForRequestedSnapshotReturned(int snapshotVersion, int firstPackageUses, int secondPackageUses, int thirdPackageUses)
        {
            this.Given(x => x.GivenADatabaseWithTwoSnapshots())
                .When(x => x.WhenGetPackageUses(snapshotVersion))
                .Then(x => x.ThenOnlyUsesForRequestedSnapshotReturned(firstPackageUses, secondPackageUses, thirdPackageUses))
                .BDDfy();
        }

        private void GivenADatabaseWithTwoSnapshots()
        {
            _package1 = new Package("First Package", "1.0", null) { Id = 1};
            _package2 = new Package("Second Package", "1.1", null) { Id = 2 };
            _package3 = new Package("Third Package", "4.0", null) { Id = 3 };
            var packagesInDb = new List<Package>()
                                   {
                                       _package1,
                                       _package2,
                                       _package3
                                   }.AsQueryable();
            // Snapshot 1 : P1, P2
            // Snapshot 2 : P3
            var projectPackagesInDb =
                new List<ProjectPackage>()
                    {
                        new ProjectPackage()
                            {
                                Package = _package1,
                                PackageId = _package1.Id,
                                SnapshotVersion = 1
                            },
                        new ProjectPackage()
                            {
                                Package = _package2,
                                PackageId = _package2.Id,
                                SnapshotVersion = 1
                            },
                        new ProjectPackage()
                            {
                                Package = _package3,
                                PackageId = _package3.Id,
                                SnapshotVersion = 2
                            }
                    }.AsQueryable();

            var packageSetMock = _automocker.GetMock<DbSet<Package>>();
            var projectPacakgesSetMock = _automocker.GetMock<DbSet<ProjectPackage>>();
            SetDbsetMock(packageSetMock, packagesInDb);
            SetDbsetMock(projectPacakgesSetMock, projectPackagesInDb);
            _databaseMock.Setup(x => x.Packages).Returns(packageSetMock.Object);
            _databaseMock.Setup(x => x.ProjectPackages).Returns(projectPacakgesSetMock.Object);
        }

        private async Task WhenGetPackageUses(int snapshotVersion)
        {
            _packageUses = await _packageRepository.GetPackageUsesAsync(snapshotVersion);
        }

        private void ThenOnlyUsesForRequestedSnapshotReturned(int firstPackageUses, int secondPackageUses, int thirdPackageUses)
        {
            _packageUses.Count.ShouldBe(3);
            _packageUses[_package1].ShouldBe(firstPackageUses);
            _packageUses[_package2].ShouldBe(secondPackageUses);
            _packageUses[_package3].ShouldBe(thirdPackageUses);
        }

        private void SetDbsetMock<TType>(Mock<DbSet<TType>> dbsetMock, IQueryable<TType> typeInDb) where TType : class
        {
            dbsetMock.As<IQueryable<TType>>().Setup(m => m.Provider).Returns(typeInDb.Provider);
            dbsetMock.As<IQueryable<TType>>().Setup(m => m.Expression).Returns(typeInDb.Expression);
            dbsetMock.As<IQueryable<TType>>().Setup(m => m.ElementType).Returns(typeInDb.ElementType);
            dbsetMock.As<IQueryable<TType>>().Setup(m => m.GetEnumerator()).Returns(typeInDb.GetEnumerator());
        }

    }
}
