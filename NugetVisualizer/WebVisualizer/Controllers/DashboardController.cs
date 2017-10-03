using Microsoft.AspNetCore.Mvc;

namespace WebVisualizer.Controllers
{
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
            var model = new DashboardViewModel() { MostUsedPackagesViewModel = _dashboardService.GetMostUsedPackagesViewModel(5) };
            return View(model);
        }

        public IActionResult MostUsed(DashboardViewModel inputModel)
        {
            var model = new DashboardViewModel() { MostUsedPackagesViewModel = _dashboardService.GetMostUsedPackagesViewModel(inputModel.MostUsedPackagesViewModel.MaxToRetrieve) };
            return View("Index", model);
        }
    }
}
