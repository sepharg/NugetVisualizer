namespace WebVisualizer.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Autofac;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using NugetVisualizer.Core;
    using NugetVisualizer.Core.Dto;

    using WebVisualizer.Models;
    using WebVisualizer.Services;

    public class HarvestController : Controller
    {
        private readonly SnapshotService _snapshotService;

        private readonly IComponentContext _context;

        private string _githubOrganization;

        public HarvestController(SnapshotService snapshotService, IComponentContext  context)
        {
            _snapshotService = snapshotService;
            _context = context;
            _githubOrganization = _context.Resolve<IConfigurationHelper>().GetConfiguration()["GithubOrganization"];
        }

        public async Task<IActionResult> Index()
        {
            var model = GetDefaultViewModel();
            if (model == null)
            {
                return RedirectToAction("CreateSnapshot", "Harvest");
            }
            return View(model);
        }

        private HarvestViewModel GetDefaultViewModel()
        {
            var snapshots = _snapshotService.GetSnapshots();

            if (snapshots.Count == 0)
            {
                return null;
            }

            return new HarvestViewModel(_githubOrganization)
                       {
                           Snapshots = snapshots.Select(s => new SelectListItem() { Text = s.Name, Value = s.Version.ToString()}).ToList()
                       };
        }

        [HttpGet]
        public async Task<ActionResult> CreateSnapshot()
        {
            //Create
            return View("Create", new HarvestViewModel(_githubOrganization));
        }

        [HttpPost]
        public async Task<ActionResult> CreateSnapshot(HarvestViewModel model)
        {
            // Append
            return View("Create", model);
        }

        [HttpPost]
        public async Task<IActionResult> DoCreateSnapshot(HarvestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var processor = _context.Resolve<IProcessor>(new TypedParameter(typeof(ProjectParserType), model.ParserType));
                string[] filters = new string[0];
                if (!string.IsNullOrWhiteSpace(model.Filters))
                {
                    filters = model.Filters.Split(' ');
                }
                ProjectParsingResult projectParsingResult;
                if (model.SelectedSnapshotId == default(int))
                {
                    projectParsingResult = await processor.Process(model.RootPath, filters, model.SnapshotName);

                }
                else
                {
                    projectParsingResult = await processor.Process(model.RootPath, filters, model.SelectedSnapshotId);
                }
                model.ProjectParsingResult = projectParsingResult;
                return View("Create", model);
            }
            return View("Create", model);
        }
    }
}
