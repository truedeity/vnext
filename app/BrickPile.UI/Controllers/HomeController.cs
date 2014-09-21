using Microsoft.AspNet.FileSystems;
using Microsoft.AspNet.Mvc;
using Raven.Client;
using Raven.Client.FileSystem;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BrickPile.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDocumentStore _store;
        private readonly IFilesStore _filesStore;
        private readonly IFileSystem _fileSystem;
        // GET: /<controller>/
        public async Task<IActionResult> Index(Home currentModel)
        {
            //return View();

            //using (var session = _store.OpenAsyncSession())
            //{
            //    var collection = new Collection { Name = "Collection 1" };
            //    await session.StoreAsync(collection);
            //    await session.SaveChangesAsync();
            //}

            //using (var session = _filesStore.OpenAsyncSession())
            //{
            //    var bla = await session.LoadFileAsync("images/hubot.jpg");

            //}

            //var provider = new BrickPileFileSystem(_filesStore);
            
            IFileInfo info;
            IEnumerable<IFileInfo> infos;
            _fileSystem.TryGetFileInfo("/images/hubot.jpg", out info);
            _fileSystem.TryGetDirectoryContents("/images", out infos);

            return new ContentResult() {
                Content = "This is the Home / Index action mother fucker!!! " + currentModel.Heading +
                string.Join(", ", infos.Select(x => x.PhysicalPath))
            };

        }

        public HomeController(IDocumentStore store, IFilesStore filesStore, IFileSystem fileSystem)
        {
            _store = store;
            _filesStore = filesStore;
            _fileSystem = fileSystem;
        }

        public IActionResult Test()
        {
            return new ContentResult() { Content = "This is the Home/Test action... " };
        }
    }
}
