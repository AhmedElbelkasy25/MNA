using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;

namespace MNA.Areas.Student.Controllers
{
    [Area("Student")]
    public class CourseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index(int? categoryId, int pageNumber = 1, int numOfItems = 9)
        {
            var query = _unitOfWork.Courses.Get(
                includeProps: q => q.Include(c => c.Category).Include(c => c.Instructor)
            );

            // Apply Category Filter
            if (categoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categoryId);
                ViewBag.CategoryName = _unitOfWork.Categories.GetOne(c => c.Id == categoryId)?.Name ?? "All Courses";
            }
            else
            {
                ViewBag.CategoryName = "All Courses";
            }

            // Pagination
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / numOfItems);
            var courses = query.Skip((pageNumber - 1) * numOfItems).Take(numOfItems).ToList();

            var coursePaginationVM = new CoursePaginationVM
            {
                Courses = courses,
                Pages = totalPages,
                PageNumber = pageNumber
            };

            return View(coursePaginationVM);
        }

        public IActionResult Details(int Id)
        {
            var course = _unitOfWork.Courses.GetOne(
                filter: e => e.Id == Id,
                includeProps: e => e.Include(e => e.Instructor)
                                    .Include(e => e.Sections)
                                    .ThenInclude(e => e.Lessons)
            );

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
    }
}
