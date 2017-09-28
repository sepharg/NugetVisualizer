using System.Collections.Generic;

namespace WebVisualizer.Services
{
    using NugetVisualizer.Core.Domain;
    using NugetVisualizer.Core.Repositories;

    public class PackageSearchService
    {
        private readonly IPackageRepository _packageRepository;


        public PackageSearchService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public List<Package> GetPackages()
        {
            return _packageRepository.LoadPackages();
        }
    }
}
