using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MNA.Areas.Instructor.Controllers
{
    public class CourseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course, IFormFile? file, IFormFile? file2)
        {
            ModelState.Remove("ImgUrl");
            ModelState.Remove("Category");
            ModelState.Remove("Instructor");
            ModelState.Remove("Preview");

            var user = await _userManager.GetUserAsync(User);
            var instructor = _unitOfWork.Instructors.GetOne(i => i.UserId == user.Id);
            if (instructor == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0 && file2 != null && file2.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var file2Name = Guid.NewGuid().ToString() + Path.GetExtension(file2.FileName);

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Course", fileName);
                    var file2Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Videos/Previews", file2Name);

                    using (var stream = System.IO.File.Create(filePath)) { file.CopyTo(stream); }
                    using (var stream = System.IO.File.Create(file2Path)) { file2.CopyTo(stream); }

                    course.ImgUrl = fileName;
                    course.preview = file2Name;
                }
                else
                {
                    ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
                    return View(course);
                }

                course.InstructorId = instructor.Id;
                // disable the trigger of courses table
                _unitOfWork.ExecuteRawSql("DISABLE TRIGGER triggerUpdateInstructorRating ON Courses");

                _unitOfWork.Courses.Create(course);
                _unitOfWork.Commit();
                _unitOfWork.ExecuteRawSql("ENABLE TRIGGER triggerUpdateInstructorRating ON Courses");

                TempData["success"] = "Course has been added successfully";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
            return View(course);
        }

        // Edit - Show edit form
        public IActionResult Edit(int id)
        {
            var course = _unitOfWork.Courses.GetOne(c => c.Id == id, includeProps: query => query.Include(c => c.Category));
            if (course == null) return NotFound();

            ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
            return View(course);
        }

        // Edit - Update course details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course, IFormFile? file, IFormFile? file2)
        {
            if (id != course.Id) return BadRequest();
            ModelState.Remove("ImgUrl");
            ModelState.Remove("Category");
            ModelState.Remove("Instructor");
            ModelState.Remove("Preview");

            var user = await _userManager.GetUserAsync(User);
            var instructor = _unitOfWork.Instructors.GetOne(i => i.UserId == user.Id);
            if (instructor == null) return Unauthorized();

            var oldCourse = _unitOfWork.Courses.GetOne(c => c.Id == course.Id, tracked: false);
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Course", fileName);

                    using (var stream = System.IO.File.Create(filePath)) { file.CopyTo(stream); }

                    if (!string.IsNullOrEmpty(oldCourse.ImgUrl))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Course", oldCourse.ImgUrl);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    course.ImgUrl = fileName;
                }
                else { course.ImgUrl = oldCourse.ImgUrl; }

                // disable the trigger of courses table
                _unitOfWork.ExecuteRawSql("DISABLE TRIGGER triggerUpdateInstructorRating ON Courses");

                _unitOfWork.Courses.Alter(course);
                _unitOfWork.Commit();
                _unitOfWork.ExecuteRawSql("ENABLE TRIGGER triggerUpdateInstructorRating ON Courses");

                TempData["success"] = "Course has been updated successfully";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
            return View(course);
        }

        // Delete - Show confirmation page
        public IActionResult Delete(int id)
        {
            var course = _unitOfWork.Courses.GetOne(c => c.Id == id, includeProps: query => query.Include(c => c.Category));
            if (course == null) return NotFound();

            return View(course);
        }

        // Delete - Confirm deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var course = _unitOfWork.Courses.GetOne(c => c.Id == id);
            if (course == null) return NotFound();

            // disable the trigger of courses table
            _unitOfWork.ExecuteRawSql("DISABLE TRIGGER triggerUpdateInstructorRating ON Courses");

            _unitOfWork.Courses.Delete(course);
            _unitOfWork.Commit();
            _unitOfWork.ExecuteRawSql("ENABLE TRIGGER triggerUpdateInstructorRating ON Courses");

            TempData["success"] = "Course has been deleted successfully";
            return RedirectToAction(nameof(Index));
        }



    }
}
