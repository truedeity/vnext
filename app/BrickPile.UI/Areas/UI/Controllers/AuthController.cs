using BrickPile.UI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using Raven.Client;
using System;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BrickPile.UI.Controllers
{
    [Area("UI"), Authorize]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var signInStatus = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
                switch (signInStatus)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        ModelState.AddModelError("", "User is locked out, try again later.");
                        return View(model);
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid username or password.");
                        return View(model);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public IActionResult Manage(ManageMessageId? message = null)
        {
            ViewBag.StatusMessage =
            message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
            : message == ManageMessageId.Error ? "An error has occurred."
            : "";
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(ManageUserViewModel model)
        {
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                else
                {
                    AddErrors(result);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //// GET: /<controller>/
        //public async Task<IActionResult> Index()
        //{

        //    //this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.documentStore),, null, null, null, null);

        //    //UserManager<ApplicationUser> userManager = serviceProvider.GetService(typeof(IUserStore<ApplicationUser>));

        //    //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.documentStore));

        //    //builder.Services.AddScoped<UserManager<TUser>, UserManager<TUser>>();

        //    //var user = new ApplicationUser { UserName = "Marcus", Email = "marcus@meridium.se" };
        //    //IdentityResult result = await userManager.CreateAsync(user, "pkrMum");

        //    var signInStatus = await signInManager.PasswordSignInAsync("Marcus", "pkrMum", false, shouldLockout: false);

        //    //if (signInStatus)
        //    //{
        //    //    //await signInManager.SignInAsync(user, isPersistent: false);
        //    //}

        //    return View();
        //}

        public IActionResult LogOff()
        {
            signInManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await userManager.FindByNameAsync(Context.User.Identity.Name);
        }
        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            Error
        }

        //public async Task<IActionResult> Login(string returnUrl)
        //{

        //    var user = new ApplicationUser { UserName = "Marcus" };
        //    var result = await UserManager.CreateAsync(user, "kUwVJ6h3");

        //    return View();
        //}


    }
}
