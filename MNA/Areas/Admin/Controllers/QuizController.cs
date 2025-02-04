using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MNA.Areas.Admin.Controllers
{


    [Area("Admin")]

    public class QuizController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuizController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            var quizzes = _unitOfWork.Quizs.Get(
                includeProps: query => query.Include(l => l.Lesson)
            ).ToList();

            return View(quizzes);
        }

        public IActionResult Create()
        {
            ViewBag.lessons = _unitOfWork.Lessons.Get().ToList();
            return View();
        }


        [HttpPost]
        public IActionResult Create(Quiz quiz)
        {
            ModelState.Remove("Lesson");
            if (ModelState.IsValid)
            {
                _unitOfWork.Quizs.Create(quiz);
                _unitOfWork.Commit();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Lessons = _unitOfWork.Lessons.Get().ToList();
            return View(quiz);
        }

        public IActionResult Edit(int id)
        {
            var quiz = _unitOfWork.Quizs.GetOne(
                l => l.Id == id,
                includeProps: query => query.Include(l => l.Lesson)
            );

            if (quiz == null)
            {
                return NotFound();
            }
            ViewBag.lessons = _unitOfWork.Lessons.Get().ToList();
            return View(quiz);
        }

        [HttpPost]
        public IActionResult Edit(int id, Quiz quiz)
        {
            if (id != quiz.Id)
            {
                return BadRequest();
            }
            ModelState.Remove("Lesson");
            if (ModelState.IsValid)
            {
                _unitOfWork.Quizs.Alter(quiz);
                _unitOfWork.Commit();
                return RedirectToAction(nameof(Index));
            }

            return View(quiz);
        }

        public IActionResult Delete(int id)
        {
            var quiz = _unitOfWork.Quizs.GetOne(
                l => l.Id == id,
                includeProps: query => query.Include(l => l.Lesson)
            );

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var quiz = _unitOfWork.Quizs.GetOne(l => l.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            _unitOfWork.Quizs.Delete(quiz);
            _unitOfWork.Commit();
            return RedirectToAction(nameof(Index));
        }
    }
}
