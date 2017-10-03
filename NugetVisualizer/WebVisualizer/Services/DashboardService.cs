using System.Collections.Generic;
using System.Linq;

namespace WebVisualizer.Services
{
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

        private Dictionary<Package, int> GetMostUsedPackages(int maxNumberToRetrieve)
        {
            var packageUses = _packageRepository.GetPackageUses();
            return packageUses.OrderByDescending(x => x.Value).Take(maxNumberToRetrieve).ToDictionary(x => x.Key, x => x.Value);
        }

        private Dictionary<Package, int> GetLeastUsedPackages(int maxNumberToRetrieve)
        {
            var packageUses = _packageRepository.GetPackageUses();
            return packageUses.OrderBy(x => x.Value).Take(maxNumberToRetrieve).ToDictionary(x => x.Key, x => x.Value);
        }

        public UsedPackagesViewModel GetLeastUsedPackagesViewModel(int maxNumberToRetrieve)
        {
            var mostUsedPackages = GetLeastUsedPackages(maxNumberToRetrieve);
            var viewModel = new LeastUsedPackagesViewModel() { MaxToRetrieve = maxNumberToRetrieve };
            return SetViewmodelValues(viewModel, mostUsedPackages);
        }

        public UsedPackagesViewModel GetMostUsedPackagesViewModel(int maxNumberToRetrieve)
        {
            var mostUsedPackages = GetMostUsedPackages(maxNumberToRetrieve);
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
