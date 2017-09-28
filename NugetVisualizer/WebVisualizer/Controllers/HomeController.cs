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
                model.Versions = new List<string>() { "1.1", "2.0", "2.1", "3.1.3"};
                model.ProjectRows = new List<ProjectRow>()
                                        {
                                            new ProjectRow()
                                                {
                                                    ProjectName = "FIRST PROJ",
                                                    ValuesList = new List<string>() { string.Empty, "X", String.Empty, String.Empty }
                                                },
                                            new ProjectRow()
                                                {
                                                    ProjectName = "SECOND PROJ",
                                                    ValuesList = new List<string>() { "X", string.Empty, String.Empty, String.Empty }
                                                },
                                            new ProjectRow()
                                                {
                                                    ProjectName = "THIRD PROJ",
                                                    ValuesList = new List<string>() { string.Empty, "X", String.Empty, String.Empty }
                                                },
                                            new ProjectRow()
                                                {
                                                    ProjectName = "FOURTH PROJ",
                                                    ValuesList = new List<string>() { string.Empty, "X", "X", String.Empty }
                                                },
                                            new ProjectRow()
                                                {
                                                    ProjectName = "FIFTH PROJ",
                                                    ValuesList = new List<string>() { string.Empty, "X", String.Empty, "X" }
                                                },
                                        };
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
