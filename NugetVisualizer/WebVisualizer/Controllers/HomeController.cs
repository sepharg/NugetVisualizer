using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using WebVisualizer.Models;

namespace WebVisualizer.Controllers
{
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
            packagesViewModel.SetPackagesOrderedByVersionCount(await _packageSearchService.GetPackagesOrderedByVersions(1));
            return View(packagesViewModel);
        }
        
        public async Task<IActionResult> ShowPackagesOrderedByVersionPackageName(PackagesViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Versions = _packageSearchService.GetPackageVersions(model.SelectedPackageName);
                model.SetPackagesOrderedByVersionCount(await _packageSearchService.GetPackagesOrderedByVersions(1));
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
