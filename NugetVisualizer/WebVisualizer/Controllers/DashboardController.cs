namespace WebVisualizer.Controllers
{
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
        public IActionResult Index()
        {
            var model = GetDefaultDashboardViewModel();
            return View(model);
        }

        private DashboardViewModel GetDefaultDashboardViewModel()
        {
            return new DashboardViewModel() { MostUsedPackagesViewModel = (MostUsedPackagesViewModel)_dashboardService.GetMostUsedPackagesViewModel(5, 1), LeastUsedPackagesViewModel = (LeastUsedPackagesViewModel)_dashboardService.GetLeastUsedPackagesViewModel(5, 1)};
        }

        public IActionResult MostUsed(int maxToRetrieve, int snapshotVersion)
        {
            var model = (MostUsedPackagesViewModel)_dashboardService.GetMostUsedPackagesViewModel(maxToRetrieve, snapshotVersion);
            return PartialView("Widgets/UsedPackages", model);
        }

        public IActionResult LeastUsed(int maxToRetrieve, int snapshotVersion)
        {
            var model = (LeastUsedPackagesViewModel)_dashboardService.GetLeastUsedPackagesViewModel(maxToRetrieve, snapshotVersion);
            return PartialView("Widgets/UsedPackages", model);
        }
    }
}
