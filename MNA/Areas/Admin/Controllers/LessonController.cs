using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MNA.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class LessonController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LessonController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Lesson
        public IActionResult Index()
        {
            // Include related Course data
            var lessons = _unitOfWork.Lessons.Get(
                includeProps: query => query.Include(l => l.Section)
            ).ToList();

            return View(lessons);
        }

        // GET: Lesson/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: Lesson/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Lessons.Create(lesson);
                return RedirectToAction(nameof(Index));
            }

            // Repopulate ViewBag in case of validation errors
            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();
            return View(lesson);
        }

        // GET: Lesson/Edit/5
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

            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();
            return View(lesson);
        }

        // POST: Lesson/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Lesson lesson)
        {
            if (id != lesson.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Lessons.Alter(lesson);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();
            return View(lesson);
        }

        // GET: Lesson/Delete/5
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
            return RedirectToAction(nameof(Index));
        }
    }
}
