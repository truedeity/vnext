using Microsoft.AspNet.Mvc;
using Raven.Client.FileSystem;
using System.IO;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BrickPile.UI.Controllers
{
    [Area("UI")]
    public class AssetsController : Controller
    {
        IFilesStore _filesStore;
        // GET: /<controller>/
        public async Task<IActionResult> Index(string folder = "/")
        {
            using (var session = _filesStore.OpenAsyncSession())
            {
                var files = await session.Query().ToListAsync();
                ViewBag.Files = files;
                var directories = await session.Commands.GetDirectoriesAsync();

                //await session.Commands.UploadAsync("/random/abc.txt", new MemoryStream());

                return View(directories);

                //return Content("Content from ui/assets/index");
            };
        }

        public AssetsController(IFilesStore filesStore)
        {
            _filesStore = filesStore;
        }
    }
}
