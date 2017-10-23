using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using WebVisualizer.Models;

namespace WebVisualizer.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using WebVisualizer.Services;

    public class HomeController : Controller
    {
        private readonly PackageSearchService _packageSearchService;

        public HomeController(PackageSearchService packageSearchService)
        {
            _packageSearchService = packageSearchService;
        }

        public async Task<IActionResult> Index()
        {
            var packagesViewModel = new PackagesViewModel();
            var snapshots = _packageSearchService.GetSnapshots();
            packagesViewModel.SetPackagesOrderedByVersionCount(await _packageSearchService.GetPackagesOrderedByVersions(snapshots.First().Version), snapshots);
            return View(packagesViewModel);
        }

        public async Task<IActionResult> ChangeSnapshot(PackagesViewModel model)
        {
            var snapshots = _packageSearchService.GetSnapshots();
            model.SetPackagesOrderedByVersionCount(await _packageSearchService.GetPackagesOrderedByVersions(model.SelectedSnapshotId), snapshots);
            return View("Index", model);
        }

        public async Task<IActionResult> ShowPackagesOrderedByVersionPackageName(PackagesViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Versions = await _packageSearchService.GetPackageVersions(model.SelectedPackageName, model.SelectedSnapshotId);
                model.SetPackagesOrderedByVersionCount(await _packageSearchService.GetPackagesOrderedByVersions(model.SelectedSnapshotId), _packageSearchService.GetSnapshots());
                model.ProjectRows = _packageSearchService.GetProjectRows(model.SelectedPackageName, model.Versions);
                return View("Index", model);
            }

            // If we got this far, something failed; redisplay form.
            return View("Index", model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
