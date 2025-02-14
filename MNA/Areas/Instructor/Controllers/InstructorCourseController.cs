using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using System.Security.Claims;

namespace MNA.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class InstructorCourseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorCourseController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index(int? categoryId, int pageNumber = 1, int numOfItems = 9)
        {
            // Get the logged-in user's ID
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            // Get the instructor linked to this user
            var instructor = _unitOfWork.Instructors.GetOne(i => i.UserId == userId);
            if (instructor == null)
            {
                return NotFound("Instructor profile not found.");
            }

            // Query courses created by this instructor
            var query = _unitOfWork.Courses.Get(
                filter: c => c.InstructorId == instructor.Id, // Filter by instructor
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



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course, IFormFile? file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var instructor = _unitOfWork.Instructors.GetOne(i => i.UserId == user.Id);
            if (instructor == null) return Unauthorized();

            course.InstructorId = instructor.Id;

            if (ModelState.IsValid)
            {
                _unitOfWork.Courses.Create(course);
                _unitOfWork.Commit();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = _unitOfWork.Instructors.GetOne(i => i.UserId == user.Id);
            if (instructor == null) return Unauthorized();

            var course = _unitOfWork.Courses.GetOne(c => c.Id == id && c.InstructorId == instructor.Id);
            if (course == null) return NotFound();

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = _unitOfWork.Instructors.GetOne(i => i.UserId == user.Id);
            if (instructor == null) return Unauthorized();

            if (id != course.Id || course.InstructorId != instructor.Id)
            {
                return BadRequest();
            }

            _unitOfWork.Courses.Alter(course);
            _unitOfWork.Commit();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = _unitOfWork.Instructors.GetOne(i => i.UserId == user.Id);
            if (instructor == null) return Unauthorized();

            var course = _unitOfWork.Courses.GetOne(c => c.Id == id && c.InstructorId == instructor.Id);
            if (course == null) return NotFound();

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = _unitOfWork.Instructors.GetOne(i => i.UserId == user.Id);
            if (instructor == null) return Unauthorized();

            var course = _unitOfWork.Courses.GetOne(c => c.Id == id && c.InstructorId == instructor.Id);
            if (course == null) return NotFound();

            _unitOfWork.Courses.Delete(course);
            _unitOfWork.Commit();

            return RedirectToAction(nameof(Index));
        }
    }
}
