using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using System.Diagnostics;

namespace MNA.Areas.Student.Controllers
{
    [Area("Student")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager ,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this._userManager = userManager;
            this._unitOfWork = unitOfWork;
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

            var courses = _unitOfWork.Courses.Get().OrderByDescending(e=>e.Id).ToList();
            var featuredCourses = _unitOfWork.Courses.Get().OrderByDescending(e => e.Rating).Take(4).ToList();
            var instructors = _unitOfWork.Instructors.Get().ToList();
            var reviews = _unitOfWork.Reviews.Get(includeProps:e=>e.Include(e=>e.Course)
            .Include(e => e.Student)).ToList();

            var coursevm = new CourseReviewInstructorVM() {
                Courses = courses,
                Instructors = instructors.ToList(),
                Reviews = reviews.ToList(),
                FeatureCourse = featuredCourses

            };

            return View(model: coursevm);
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
