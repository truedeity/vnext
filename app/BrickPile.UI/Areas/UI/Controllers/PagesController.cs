using Microsoft.AspNet.Mvc;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BrickPile.UI.Controllers
{
    [Area("ui")]
    [Route("[area]/pages")]
    public class PagesController : Controller
    {
        private List<Type> _availableModels = new List<Type>();
        private readonly IDocumentStore _documentStore;

        public PagesController(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        // GET: /<controller>/
        [HttpGet("{page?}")]
        public async Task<IActionResult> Index(string page = "1")
        {
            dynamic p;
            using(var session = _documentStore.OpenAsyncSession())
            {
                ViewBag.Pages = await session.Query<Page>().ToListAsync();
                p = await session.LoadAsync<dynamic>("pages/" + page + "/content");
            }

            return View(p);
        }

        public async Task<IActionResult> Save(string page)
        {
            dynamic model;
            using (var session = DefaultBrickPileBootstrapper.DocumentStore.OpenAsyncSession())
            {
                model = await session.LoadAsync<dynamic>("pages/" + page + "/content");

                bool result = await TryUpdateModelAsync(model);

                if (!result)
                {
                    return View("Index", model);
                }

                await session.StoreAsync(model);
                await session.SaveChangesAsync();

            }
            return View("Index", model);
            //return Json(model);
        }
    }
}
