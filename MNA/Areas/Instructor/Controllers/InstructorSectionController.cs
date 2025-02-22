using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MNA.Areas.Instructor.Controllers
{

    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class InstructorSectionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorSectionController(IUnitOfWork unitOfWork , UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            this._userManager = userManager;
        }

        public IActionResult Create(int? sectionId)
        {
            
            ViewBag.Sections = _unitOfWork.Sections.Get(filter: e => e.Id == sectionId).ToList();
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Lesson lesson, IFormFile? file)
        {
            ModelState.Remove("Section");
            ModelState.Remove("Content");

            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Videos", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }

                    lesson.Content = fileName;
                }
                else
                {
                    ViewBag.Sections = _unitOfWork.Sections.Get(filter: e => e.Id == lesson.SectionId).ToList();
                    return View(lesson);
                }

                _unitOfWork.Lessons.Create(lesson);
                _unitOfWork.Commit();

                TempData["success"] = "Lesson has been created successfully";
                return RedirectToAction("Index", "InstructorCourse");
            }

            ViewBag.Sections = _unitOfWork.Sections.Get().ToList();
            return View(lesson);
        }



        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = _unitOfWork.Instructors.GetOne(i => i.UserId == user.Id);
            if (instructor == null) return Unauthorized();

            // Ensure the section belongs to an instructor's course
            var section = _unitOfWork.Sections.GetOne(
                s => s.Id == id && s.Course.InstructorId == instructor.Id,
                includeProps: query => query.Include(s => s.Course)
            );
            if (section == null) return NotFound();

            // Populate ViewBag.Courses with the instructor's courses
            ViewBag.Courses = _unitOfWork.Courses.Get(c => c.InstructorId == instructor.Id);

            return View(section);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Section section)
        {
            if (id != section.Id) return BadRequest();

            ModelState.Remove("Course");
            ModelState.Remove("Lessons");

            var user = await _userManager.GetUserAsync(User);
            var instructor = _unitOfWork.Instructors.GetOne(i => i.UserId == user.Id);
            if (instructor == null) return Unauthorized();

            // Fetch the existing section to prevent EF tracking conflicts
            var existingSection = _unitOfWork.Sections.GetOne(s => s.Id == id && s.Course.InstructorId == instructor.Id);
            if (existingSection == null) return Forbid();

            if (ModelState.IsValid)
            {
                // Update only the properties that were changed
                existingSection.Title = section.Title;


                _unitOfWork.Sections.Alter(existingSection); // Use the existing tracked entity
                _unitOfWork.Commit();

                return RedirectToAction("Index", "InstructorCourse");
            }

            // Repopulate ViewBag.Courses in case of validation errors
            ViewBag.Courses = _unitOfWork.Courses.Get(c => c.InstructorId == instructor.Id);

            return View(section);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = _unitOfWork.Instructors.GetOne(i => i.UserId == user.Id);
            if (instructor == null) return Unauthorized();

            // Ensure the section belongs to the instructor
            var section = _unitOfWork.Sections.GetOne(
                s => s.Id == id && s.Course.InstructorId == instructor.Id,
                includeProps: query => query.Include(s => s.Course)
            );
            if (section == null) return NotFound();

            return View(section);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = _unitOfWork.Instructors.GetOne(i => i.UserId == user.Id);
            if (instructor == null) return Unauthorized();

            // Ensure the section belongs to the instructor
            var section = _unitOfWork.Sections.GetOne(s => s.Id == id && s.Course.InstructorId == instructor.Id);
            if (section == null) return Forbid();

            _unitOfWork.Sections.Delete(section);
            _unitOfWork.Commit();
            return RedirectToAction("Index", "InstructorCourse");
        }
    }
}
