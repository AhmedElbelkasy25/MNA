using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq;

namespace MNA.Areas.Student.Controllers
{
    [Area("student")]
    public class EnrollmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollmentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult MyCourses()
        {
            var applicationUserId = _userManager.GetUserId(User);

            if (applicationUserId == null)
            {
                TempData["error"] = "You must be logged in to view your courses.";
                return RedirectToAction("Index", "Home");
            }

            var enrolledCourses = _unitOfWork.Enrollments.Get(
                e => e.ApplicationUserId == applicationUserId,
                includeProps: q => q.Include(e => e.Course) // Load course details
            ).Select(e => e.Course).ToList();

            return View(enrolledCourses);
        }

    }
}
