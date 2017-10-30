namespace WebVisualizer.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using WebVisualizer.Models;
    using WebVisualizer.Services;

    public class HomeController : Controller
    {
        private readonly PackageSearchService _packageSearchService;

        private readonly SnapshotService _snapshotService;

        public HomeController(PackageSearchService packageSearchService, SnapshotService snapshotService)
        {
            _packageSearchService = packageSearchService;
            _snapshotService = snapshotService;
        }

        public async Task<IActionResult> Index()
        {
            var packagesViewModel = new PackagesViewModel();
            var snapshots = _snapshotService.GetSnapshots();
            if (snapshots.Count == 0)
            {
                return RedirectToAction("CreateSnapshot", "Harvest");
            }
            packagesViewModel.SetPackagesOrderedByVersionCount(await _packageSearchService.GetPackagesOrderedByVersions(snapshots.First().Version), snapshots);

            return View(packagesViewModel);
        }

        public async Task<IActionResult> ChangeSnapshot(PackagesViewModel model)
        {
            var snapshots = _snapshotService.GetSnapshots();
            model.SetPackagesOrderedByVersionCount(await _packageSearchService.GetPackagesOrderedByVersions(model.SelectedSnapshotId), snapshots);
            return View("Index", model);
        }

        public async Task<IActionResult> ShowPackagesOrderedByVersionPackageName(PackagesViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Versions = await _packageSearchService.GetPackageVersions(model.SelectedPackageName, model.SelectedSnapshotId);
                model.SetPackagesOrderedByVersionCount(await _packageSearchService.GetPackagesOrderedByVersions(model.SelectedSnapshotId), _snapshotService.GetSnapshots());
                model.ProjectRows = await _packageSearchService.GetProjectRows(model.SelectedPackageName, model.Versions, model.SelectedSnapshotId);
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
