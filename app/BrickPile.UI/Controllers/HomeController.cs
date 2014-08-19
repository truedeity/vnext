using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BrickPile.UI.Controllers
{
    public class HomeController
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return new ContentResult() { Content = "This is the Home / Index action." };
        }
    }
}
