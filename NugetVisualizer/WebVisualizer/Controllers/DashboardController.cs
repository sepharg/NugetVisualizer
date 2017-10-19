namespace WebVisualizer.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using WebVisualizer.Models;
    using WebVisualizer.Services;

    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var model = await GetDefaultDashboardViewModel();
            return View(model);
        }

        private async Task<DashboardViewModel> GetDefaultDashboardViewModel()
        {
            return new DashboardViewModel() { MostUsedPackagesViewModel = (MostUsedPackagesViewModel)await _dashboardService.GetMostUsedPackagesViewModel(5, 2), LeastUsedPackagesViewModel = (LeastUsedPackagesViewModel)await _dashboardService.GetLeastUsedPackagesViewModel(5, 1)};
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
