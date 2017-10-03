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
            var mostUsedPackages = _packageRepository.GetMostUsedPackages(maxNumberToRetrieve);
            return mostUsedPackages;
        }

        public MostUsedPackagesViewModel GetMostUsedPackagesViewModel(int maxNumberToRetrieve)
        {
            var mostUsedPackages = GetMostUsedPackages(maxNumberToRetrieve);
            var mostUsedPackagesViewModel = new MostUsedPackagesViewModel() { MaxToRetrieve = maxNumberToRetrieve };
            mostUsedPackagesViewModel.SetUsagesValues(mostUsedPackages.Select(x => x.Value).ToArray());
            mostUsedPackagesViewModel.SetPackageList(mostUsedPackages.Select(x => x.Key.Name).ToArray());
            return mostUsedPackagesViewModel;
        }
    }
}
