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
            return new DashboardViewModel() { MostUsedPackagesViewModel = (MostUsedPackagesViewModel)_dashboardService.GetMostUsedPackagesViewModel(5), LeastUsedPackagesViewModel = (LeastUsedPackagesViewModel)_dashboardService.GetLeastUsedPackagesViewModel(5)};
        }

        public IActionResult MostUsed(int maxToRetrieve)
        {
            var model = (MostUsedPackagesViewModel)_dashboardService.GetMostUsedPackagesViewModel(maxToRetrieve);
            return PartialView("Widgets/UsedPackages", model);
        }

        public IActionResult LeastUsed(int maxToRetrieve)
        {
            var model = (LeastUsedPackagesViewModel)_dashboardService.GetLeastUsedPackagesViewModel(maxToRetrieve);
            return PartialView("Widgets/UsedPackages", model);
        }
    }
}
