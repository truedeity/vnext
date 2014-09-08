using Microsoft.AspNet.Mvc;
using Raven.Client;
using Raven.Client.FileSystem;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BrickPile.UI.Controllers
{
    [Area("UI")]
    public class AssetsController : Controller
    {
        IFilesStore _filesStore;
        IDocumentStore _documentStore;

        // GET: /<controller>/
        public async Task<IActionResult> Index(string folder)
        {
            using (var session = _filesStore.OpenAsyncSession())
            {
                var files = await session.Query()
                    .OnDirectory(folder, true)
                    .ToListAsync();

                return View(files);
            };
        }

        public async Task<IActionResult> Collections(string collection)
        {
            using (var session = _filesStore.OpenAsyncSession())
            {
                var files = await session.Query()
                    .WhereEquals(x => x.Metadata["Collection"], "collections/" + collection)
                    .ToListAsync();

                return View("Index", files);
            };
        }

        public AssetsController(IFilesStore filesStore, IDocumentStore documentStore)
        {
            _filesStore = filesStore;
            _documentStore = documentStore;
        }
    }
}
