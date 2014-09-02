using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BrickPile.UI.Areas.UI.Controllers
{
    [Area("UI"), Authorize]
    public class DashboardController : Controller
    {
        private List<Type> _availableModels = new List<Type>();

        // GET: /<controller>/
        public IActionResult Index()
        {

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(ModelAttribute), true).Length > 0)
                    {
                        _availableModels.Add(type);
                    }
                }
            }

            dynamic model = Activator.CreateInstance(Type.GetType(_availableModels[0].AssemblyQualifiedName));
            return View(model);
            //return Content("This is the UI/Home/Index action.");
        }
        
        public IActionResult Test()
        {
            return Content("This is the UI/Home/Test action.");
        }

    }
}
