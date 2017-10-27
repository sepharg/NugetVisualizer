using System.Collections.Generic;
using System.Linq;

namespace WebVisualizer.Services
{
    using System.Threading.Tasks;

    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    using WebVisualizer.Models;

    public class DashboardService
    {
        private readonly IPackageRepository _packageRepository;

        public DashboardService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        private async Task<Dictionary<Package, int>> GetMostUsedPackages(int maxNumberToRetrieve, int snapshotVersion)
        {
            var packageUses = await _packageRepository.GetPackageUsesAsync(snapshotVersion);
            return packageUses.OrderByDescending(x => x.Value).Take(maxNumberToRetrieve).ToDictionary(x => x.Key, x => x.Value);
        }

        private async Task<Dictionary<Package, int>> GetLeastUsedPackages(int maxNumberToRetrieve, int snapshotVersion)
        {
            var packageUses = await _packageRepository.GetPackageUsesAsync(snapshotVersion);
            return packageUses.OrderBy(x => x.Value).Take(maxNumberToRetrieve).ToDictionary(x => x.Key, x => x.Value);
        }

        public async Task<UsedPackagesViewModel> GetLeastUsedPackagesViewModel(int maxNumberToRetrieve, int snapshotVersion)
        {
            var mostUsedPackages = await GetLeastUsedPackages(maxNumberToRetrieve, snapshotVersion);
            var viewModel = new LeastUsedPackagesViewModel() { MaxToRetrieve = maxNumberToRetrieve };
            return SetViewmodelValues(viewModel, mostUsedPackages);
        }

        public async Task<UsedPackagesViewModel> GetMostUsedPackagesViewModel(int maxNumberToRetrieve, int snapshotVersion)
        {
            var mostUsedPackages = await GetMostUsedPackages(maxNumberToRetrieve, snapshotVersion);
            var viewModel = new MostUsedPackagesViewModel() { MaxToRetrieve = maxNumberToRetrieve };
            return SetViewmodelValues(viewModel, mostUsedPackages);
        }

        private  UsedPackagesViewModel SetViewmodelValues(UsedPackagesViewModel usedPackagesViewModel, Dictionary<Package, int> mostUsedPackages)
        {
            usedPackagesViewModel.SetUsagesValues(mostUsedPackages.Select(x => x.Value).ToArray());
            usedPackagesViewModel.SetPackageList(mostUsedPackages.Select(x => x.Key.Name).ToArray());
            return usedPackagesViewModel;
        }
    }
}
