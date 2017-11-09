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

        private readonly ProjectSearchService _projectSearchService;

        private readonly SnapshotService _snapshotService;

        public HomeController(PackageSearchService packageSearchService, ProjectSearchService projectSearchService, SnapshotService snapshotService)
        {
            _packageSearchService = packageSearchService;
            _projectSearchService = projectSearchService;
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
            var snapshotVersion = snapshots.First().Version;
            packagesViewModel.SetDropdowns(await _packageSearchService.GetPackagesOrderedByVersions(snapshotVersion), await _projectSearchService.GetProjects(snapshotVersion), snapshots);

            return View(packagesViewModel);
        }

        public async Task<IActionResult> ChangeSnapshot(PackagesViewModel model)
        {
            var snapshots = _snapshotService.GetSnapshots();
            model.SetDropdowns(await _packageSearchService.GetPackagesOrderedByVersions(model.SelectedSnapshotId), await _projectSearchService.GetProjects(model.SelectedSnapshotId), snapshots);
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> ShowPackagesOrderedByVersionPackageName(PackagesViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.SearchPackagesViewModel.Versions = await _packageSearchService.GetPackageVersions(model.SearchPackagesViewModel.SelectedPackageName, model.SelectedSnapshotId);
                model.SetDropdowns(await _packageSearchService.GetPackagesOrderedByVersions(model.SelectedSnapshotId), await _projectSearchService.GetProjects(model.SelectedSnapshotId), _snapshotService.GetSnapshots());
                model.SearchPackagesViewModel.ProjectRows = await _packageSearchService.GetProjectRows(model.SearchPackagesViewModel.SelectedPackageName, model.SearchPackagesViewModel.Versions, model.SelectedSnapshotId);
                return View("Index", model);
            }

            // If we got this far, something failed; redisplay form.
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> ShowPackagesForProject(PackagesViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.SetDropdowns(await _packageSearchService.GetPackagesOrderedByVersions(model.SelectedSnapshotId), await _projectSearchService.GetProjects(model.SelectedSnapshotId), _snapshotService.GetSnapshots());
                model.SearchProjectsViewModel.PackagesForSelectedProject = _packageSearchService.GetPackagesForProject(model.SearchProjectsViewModel.SelectedProjectName, model.SelectedSnapshotId);
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
