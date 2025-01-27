using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Diagnostics;

namespace MNA.Areas.Student.Controllers
{
    [Area("Student")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this._userManager = userManager;
        }

        public async Task<IActionResult>  Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
               var role= await _userManager.GetRolesAsync(user);
                if (role.Contains("Admin"))
                {
                    return RedirectToAction("Index" , "Home" , new {Area="Admin"});
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult NotFoundPage()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
