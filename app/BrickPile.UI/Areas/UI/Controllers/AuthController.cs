using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BrickPile.UI.Controllers
{
    [Area("UI")]
    public class AuthController : Controller
    {
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public SignInManager<ApplicationUser> SignInManager { get; private set; }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {

            var user = new ApplicationUser { UserName = "Marcus" };
            var result = await UserManager.CreateAsync(user, "kUwVJ6h3");

            return View();
        }

        public async Task<IActionResult> Login(string returnUrl)
        {

            var user = new ApplicationUser { UserName = "Marcus" };
            var result = await UserManager.CreateAsync(user, "kUwVJ6h3");

            return View();
        }


    }
}
