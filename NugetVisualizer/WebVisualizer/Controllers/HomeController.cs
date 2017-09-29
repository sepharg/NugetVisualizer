using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using WebVisualizer.Models;

namespace WebVisualizer.Controllers
{
    using System;
    using System.Collections.Generic;

    using WebVisualizer.Services;

    public class HomeController : Controller
    {
        private readonly PackageSearchService _packageSearchService;

        public HomeController(PackageSearchService packageSearchService)
        {
            _packageSearchService = packageSearchService;
        }

        public IActionResult Index()
        {
            var packagesViewModel = new PackagesViewModel(_packageSearchService.GetPackages());
            packagesViewModel.SetPackagesOrderedByVersionCount(_packageSearchService.GetPackagesOrderedByVersions());
            return View(packagesViewModel);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [HttpPost]
        public IActionResult ShowPackages(PackagesViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Versions = _packageSearchService.GetPackageVersions(model.SelectedPackageName);
                model.SetPackages(_packageSearchService.GetPackages());
                model.SetPackagesOrderedByVersionCount(_packageSearchService.GetPackagesOrderedByVersions());
                model.ProjectRows = _packageSearchService.GetProjectRows(model.SelectedPackageName, model.Versions);
                
                return View("Index", model);
            }

            // If we got this far, something failed; redisplay form.
            return View("Index", model);
        }

        public IActionResult ShowPackagesOrderedByVersionPackageName(PackagesViewModel model)
        {
            model.Versions = _packageSearchService.GetPackageVersions(model.SelectedOrderedByVersionPackageName);
            model.SetPackages(_packageSearchService.GetPackages());
            model.SetPackagesOrderedByVersionCount(_packageSearchService.GetPackagesOrderedByVersions());
            model.ProjectRows = _packageSearchService.GetProjectRows(model.SelectedOrderedByVersionPackageName, model.Versions);

            return View("Index", model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
