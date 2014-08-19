using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BrickPile.UI.Areas.UI.Controllers
{
    [Area("UI")]
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
            //return Content("This is the UI/Home/Index action." + this.Url.Action("Text"));
        }

        [Area("UI")]
        public IActionResult Test()
        {
            return Content("This is the UI/Home/Test action.");
        }
    }
}
