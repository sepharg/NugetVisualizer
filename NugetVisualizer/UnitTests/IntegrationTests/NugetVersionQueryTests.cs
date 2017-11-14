using TestStack.BDDfy;
namespace UnitTests.IntegrationTests
{
    using System;
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Nuget;

    using Shouldly;

    using Xunit;

    public class NugetVersionQueryTests
    {
        private string _packageName;

        private NugetVersionQuery _nugetVersionQuery;

        private string _latestVersion;

        public NugetVersionQueryTests()
        {
            _nugetVersionQuery = new NugetVersionQuery();
        }

        [Fact]

        public void GivenAnExistingPackage_WhenGettingLatestVersion_ThenLatestVersionForPackageReturned()
        {
            this.Given(x => x.GivenAnExistingPackage())
                .When(x => x.WhenGettingLatestVersion())
                .Then(x => x.ThenLatestVersionForPackageReturned())
                .BDDfy();
        }

        [Fact]

        public void GivenAnInexistingPackage_WhenGettingLatestVersion_ThenLatestVersionForPackageReturned()
        {
            this.Given(x => x.GivenAnInxistingPackage())
                .When(x => x.WhenGettingLatestVersion())
                .Then(x => x.ThenEmptyReturned())
                .BDDfy();
        }

        private void GivenAnExistingPackage()
        {
            _packageName = "NuGet.Versioning";
        }

        private void GivenAnInxistingPackage()
        {
            _packageName = "IDONOTEXISTANDIHOPENOBODYEVERCREATESME";
        }

        private async Task WhenGettingLatestVersion()
        {
            _latestVersion = await _nugetVersionQuery.GetLatestVersion(_packageName);
        }

        private void ThenLatestVersionForPackageReturned()
        {
            _latestVersion.ShouldNotBe(NugetVersionQuery.NOVERSIONFOUND);
        }

        private void ThenEmptyReturned()
        {
            _latestVersion.ShouldBe(NugetVersionQuery.NOVERSIONFOUND);
        }
    }
}
