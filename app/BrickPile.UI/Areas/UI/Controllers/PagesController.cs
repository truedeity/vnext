using Microsoft.AspNet.Mvc;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BrickPile.UI.Controllers
{
    [Area("UI")]
    public class PagesController : Controller
    {

        private List<Type> _availableModels = new List<Type>();
        private readonly IDocumentStore _documentStore;

        public PagesController(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            using(var session = _documentStore.OpenAsyncSession())
            {
                ViewBag.Pages = await session.Query<Page>().ToListAsync();
            }
            return View();
        }

        public async Task<IActionResult> Save(TrieNode currentNode)
        {
            dynamic model;
            using (var session = DefaultBrickPileBootstrapper.DocumentStore.OpenAsyncSession())
            {
                model = await session.LoadAsync<dynamic>(currentNode.ContentId);

                bool result = await TryUpdateModelAsync(model);

                if (!result)
                {
                    return View("~/Areas/UI/Views/Home/Index.cshtml", model);
                }

                await session.SaveChangesAsync();

            }
            return Json(model);
        }
    }
}
