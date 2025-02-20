using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Stripe;
using static System.Collections.Specialized.BitVector32;

namespace MNA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LessonController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LessonController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Lesson
        //public IActionResult Index()
        //{
        //    // Include related Course data
        //    var lessons = _unitOfWork.Lessons.Get(
        //        includeProps: query => query.Include(l => l.Section)
        //    ).ToList();

        //    return View(lessons);
        //}

        // GET: Lesson/Create
        public IActionResult Create(int? sectionId)
        {
            if (sectionId == null)
            {
                ViewBag.Sections = _unitOfWork.Sections.Get().ToList();
            }
            else
            {
                ViewBag.Sections = _unitOfWork.Sections.Get(filter:e=>e.Id == sectionId).ToList();
            }
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
                    // Genereate name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Save in wwwroot
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Videos", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }

                    // Save in db
                    lesson.Content = fileName;
                }
                else
                {
                    ViewBag.Sections = _unitOfWork.Sections.Get(filter: e => e.Id == lesson.SectionId).ToList();
                    return View(lesson);
                }
                _unitOfWork.Lessons.Create(lesson);
                _unitOfWork.Commit();

                TempData["success"] = "Lesson has been Created successfully";
                return RedirectToAction("Index", "Course");
            }

           
            ViewBag.Sections = _unitOfWork.Sections.Get().ToList();
            return View(lesson);
        }

        
        public IActionResult Edit(int id)
        {
            var lesson = _unitOfWork.Lessons.GetOne(
                l => l.Id == id,
                includeProps: query => query.Include(l => l.Section)
            );

            if (lesson == null)
            {
                return NotFound();
            }

            ViewBag.Sections = _unitOfWork.Sections.Get().ToList();
            return View(lesson);
        }

        // POST: Lesson/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Lesson lesson , IFormFile? file)
        {
            if (id != lesson.Id)
            {
                return BadRequest();
            }
            ModelState.Remove("Section");
            ModelState.Remove("Content");
            var oldLesson = _unitOfWork.Lessons.GetOne(e => e.Id == lesson.Id, tracked: false);
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Genereate name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Save in wwwroot
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Videos", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    //delete old img
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Videos", oldLesson.Content);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                    // Save in db
                    lesson.Content = fileName;
                }
                else
                {
                    lesson.Content = oldLesson.Content; 
                }

                _unitOfWork.Lessons.Alter(lesson);
                _unitOfWork.Commit();
                TempData["success"] = "Lesson has been Edited successfully";
                return RedirectToAction("Index", "Course");
            }

            ViewBag.Sections = _unitOfWork.Sections.Get().ToList();
            return View(lesson);
        }

       
        public IActionResult Delete(int id)
        {
            var lesson = _unitOfWork.Lessons.GetOne(
                l => l.Id == id,
                includeProps: query => query.Include(l => l.Section)
            );

            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: Lesson/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var lesson = _unitOfWork.Lessons.GetOne(l => l.Id == id);

            if (lesson == null)
            {
                return NotFound();
            }

            _unitOfWork.Lessons.Delete(lesson);
            _unitOfWork.Commit();
            TempData["success"] = "Lesson has been Deleted successfully";
            return RedirectToAction("Index", "Course");
        }
    }
}
