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
            var model = new DashboardViewModel() { MostUsedPackagesViewModel = (MostUsedPackagesViewModel)_dashboardService.GetMostUsedPackagesViewModel(5), LeastUsedPackagesViewModel = (LeastUsedPackagesViewModel)_dashboardService.GetLeastUsedPackagesViewModel(5)};
            return View(model);
        }

        public IActionResult MostUsed(DashboardViewModel inputModel)
        {
            // this is horrible, need a better solution
            inputModel.MostUsedPackagesViewModel = (MostUsedPackagesViewModel)_dashboardService.GetMostUsedPackagesViewModel(inputModel.MostUsedPackagesViewModel.MaxToRetrieve);
            inputModel.LeastUsedPackagesViewModel = (LeastUsedPackagesViewModel)_dashboardService.GetLeastUsedPackagesViewModel(5);
            return View("Index", inputModel);
        }

        public IActionResult LeastUsed(DashboardViewModel inputModel)
        {
            // this is horrible, need a better solution
            inputModel.MostUsedPackagesViewModel = (MostUsedPackagesViewModel)_dashboardService.GetMostUsedPackagesViewModel(5);
            inputModel.LeastUsedPackagesViewModel = (LeastUsedPackagesViewModel)_dashboardService.GetLeastUsedPackagesViewModel(inputModel.LeastUsedPackagesViewModel.MaxToRetrieve);
            return View("Index", inputModel);
        }
    }
}
