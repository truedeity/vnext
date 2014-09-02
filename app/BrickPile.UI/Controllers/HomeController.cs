using Microsoft.AspNet.Mvc;
using Raven.Abstractions.FileSystem;
using Raven.Client;
using Raven.Client.FileSystem;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BrickPile.UI.Controllers
{
    public class HomeController
    {
        private readonly IDocumentStore _store;
        // GET: /<controller>/
        public async Task<IActionResult> Index(Home currentModel)
        {
            //return View();
            return new ContentResult() { Content = "This is the Home / Index action." + currentModel.Heading };

        }

        public HomeController(IDocumentStore store)
        {
            _store = store;
        }
    }
}
