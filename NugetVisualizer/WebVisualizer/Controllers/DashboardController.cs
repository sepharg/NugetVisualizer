namespace WebVisualizer.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using WebVisualizer.Models;
    using WebVisualizer.Services;

    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        private readonly SnapshotService _snapshotService;

        public DashboardController(DashboardService dashboardService, SnapshotService snapshotService)
        {
            _dashboardService = dashboardService;
            _snapshotService = snapshotService;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var model = await GetDefaultDashboardViewModel();
            return View(model);
        }

        private async Task<DashboardViewModel> GetDefaultDashboardViewModel()
        {
            var snapshots = _snapshotService.GetSnapshots();
            var mostUsedPackagesViewModel = await _dashboardService.GetMostUsedPackagesViewModel(5, snapshots.First().Version);
            var leastUsedPackagesViewModel = await _dashboardService.GetLeastUsedPackagesViewModel(5, snapshots.First().Version);
            return new DashboardViewModel()
                       {
                           MostUsedPackagesViewModel = (MostUsedPackagesViewModel)mostUsedPackagesViewModel,
                           LeastUsedPackagesViewModel = (LeastUsedPackagesViewModel)leastUsedPackagesViewModel,
                           Snapshots = snapshots.Select(s => new SelectListItem() { Text = s.Name, Value = s.Version.ToString() }).ToList() 
                       };
        }

        public async Task<IActionResult> ChangeSnapshot(DashboardViewModel model)
        {
            model.Snapshots = _snapshotService.GetSnapshots().Select(s => new SelectListItem() { Text = s.Name, Value = s.Version.ToString() }).ToList();
            model.MostUsedPackagesViewModel = (MostUsedPackagesViewModel)await _dashboardService.GetMostUsedPackagesViewModel(5, model.SelectedSnapshotId);
            model.LeastUsedPackagesViewModel = (LeastUsedPackagesViewModel)await _dashboardService.GetLeastUsedPackagesViewModel(5, model.SelectedSnapshotId);
            return View("Index", model);
        }

        public async Task<IActionResult> MostUsed(int maxToRetrieve, int snapshotVersion)
        {
            var model = (MostUsedPackagesViewModel)await _dashboardService.GetMostUsedPackagesViewModel(maxToRetrieve, snapshotVersion);
            return PartialView("Widgets/UsedPackages", model);
        }

        public async Task<IActionResult> LeastUsed(int maxToRetrieve, int snapshotVersion)
        {
            var model = (LeastUsedPackagesViewModel)await _dashboardService.GetLeastUsedPackagesViewModel(maxToRetrieve, snapshotVersion);
            return PartialView("Widgets/UsedPackages", model);
        }
    }
}
