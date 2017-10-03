using Microsoft.AspNetCore.Mvc;

namespace WebVisualizer.Controllers
{
    using WebVisualizer.Models;

    public class DashboardController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            var mostUsedPackagesViewModel = new MostUsedPackagesViewModel();
            mostUsedPackagesViewModel.SetPackageList(new[] { "uno", "dos", "tres" });
            mostUsedPackagesViewModel.SetUsagesValues(new [] { 33, 12, 20 });
            var model = new DashboardViewModel() { MostUsedPackagesViewModel = mostUsedPackagesViewModel };

            return View(model);
        }
    }
}
