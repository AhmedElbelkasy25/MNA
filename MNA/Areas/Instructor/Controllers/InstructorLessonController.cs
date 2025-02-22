using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Security.Claims;

namespace MNA.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class LessonController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LessonController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Create(int? sectionId)
        {
            var instructorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (sectionId == null)
            {
                ViewBag.Sections = _unitOfWork.Sections.Get(filter: s => s.Course.InstructorId == instructorId).ToList();
            }
            else
            {
                ViewBag.Sections = _unitOfWork.Sections.Get(filter: s => s.Id == sectionId && s.Course.InstructorId == instructorId).ToList();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Lesson lesson, IFormFile? file)
        {
            var instructorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var section = _unitOfWork.Sections.GetOne(s => s.Id == lesson.SectionId && s.Course.InstructorId == instructorId);

            if (section == null)
                return Unauthorized();

            ModelState.Remove("Section");
            ModelState.Remove("Content");

            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Videos", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }

                    lesson.Content = fileName;
                }

                _unitOfWork.Lessons.Create(lesson);
                _unitOfWork.Commit();

                TempData["success"] = "Lesson has been created successfully";
                return RedirectToAction("Index", "Course");
            }

            ViewBag.Sections = _unitOfWork.Sections.Get(filter: s => s.Course.InstructorId == instructorId).ToList();
            return View(lesson);
        }

        public IActionResult Edit(int id)
        {
            var instructorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var lesson = _unitOfWork.Lessons.GetOne(l => l.Id == id && l.Section.Course.InstructorId == instructorId,
                includeProps: query => query.Include(l => l.Section));

            if (lesson == null)
                return NotFound();

            ViewBag.Sections = _unitOfWork.Sections.Get(filter: s => s.Course.InstructorId == instructorId).ToList();
            return View(lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Lesson lesson, IFormFile? file)
        {
            var instructorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var existingLesson = _unitOfWork.Lessons.GetOne(l => l.Id == id && l.Section.Course.InstructorId == instructorId, tracked: false);

            if (existingLesson == null)
                return Unauthorized();

            ModelState.Remove("Section");
            ModelState.Remove("Content");

            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Videos", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }

                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Videos", existingLesson.Content);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                    lesson.Content = fileName;
                }
                else
                {
                    lesson.Content = existingLesson.Content;
                }

                _unitOfWork.Lessons.Alter(lesson);
                _unitOfWork.Commit();

                TempData["success"] = "Lesson has been updated successfully";
                return RedirectToAction("Index", "Course");
            }

            ViewBag.Sections = _unitOfWork.Sections.Get(filter: s => s.Course.InstructorId == instructorId).ToList();
            return View(lesson);
        }

        public IActionResult Delete(int id)
        {
            var instructorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var lesson = _unitOfWork.Lessons.GetOne(l => l.Id == id && l.Section.Course.InstructorId == instructorId,
                includeProps: query => query.Include(l => l.Section));

            if (lesson == null)
                return NotFound();

            return View(lesson);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var instructorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var lesson = _unitOfWork.Lessons.GetOne(l => l.Id == id && l.Section.Course.InstructorId == instructorId);

            if (lesson == null)
                return Unauthorized();

            _unitOfWork.Lessons.Delete(lesson);
            _unitOfWork.Commit();

            TempData["success"] = "Lesson has been deleted successfully";
            return RedirectToAction("Index", "Course");
        }
    }
}
