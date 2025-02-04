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
                includeProps: query => query.Include(q => q.Lesson)
                                            .Include(q => q.Questions)
            ).ToList();

            return View(quizzes);
        }


        public IActionResult Create()
        {
            ViewBag.Lessons = _unitOfWork.Lessons.Get().ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Quiz quiz)
        {
            

            if (!quiz.Questions.Any())
            {
                ModelState.AddModelError("", "you must add atleast one question");
            }
            ModelState.Remove("Lesson");
            for(int i=0;i<quiz.Questions.Count;i++)
            {
                ModelState.Remove($"Questions[{i}].Quiz");
            }
            if (ModelState.IsValid)
            {
                foreach (var question in quiz.Questions)
                {
                    question.QuizId = quiz.Id; 
                }

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
                q => q.Id == id,
                includeProps: e => e.Include(q => q.Lesson)
                                            .Include(q => q.Questions) 
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
            if (id != quiz.Id) return BadRequest();

            ModelState.Remove("Lesson");
            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                ModelState.Remove($"Questions[{i}].Quiz");
            }

            if (ModelState.IsValid)
            {
                var existingQuiz = _unitOfWork.Quizs.GetOne(
                    q => q.Id == id,
                    includeProps: e => e.Include(q => q.Questions)
                );

                if (existingQuiz == null) return NotFound();

                // Update lesson
                existingQuiz.LessonId = quiz.LessonId;

                // Track existing questions
                var existingQuestions = existingQuiz.Questions.ToList();
                var incomingQuestionIds = quiz.Questions.Where(q => q.Id != 0).Select(q => q.Id).ToList();

                // Remove deleted questions
                foreach (var existingQuestion in existingQuestions.ToList())
                {
                    if (!incomingQuestionIds.Contains(existingQuestion.Id))
                    {
                        existingQuiz.Questions.Remove(existingQuestion);
                        _unitOfWork.Questions.Delete(existingQuestion); 
                        
                    }
                }

                // Update or add questions
                foreach (var newQuestion in quiz.Questions)
                {
                    var existingQuestion = existingQuestions.FirstOrDefault(e => e.Id == newQuestion.Id);

                    if (existingQuestion != null)
                    {
                        existingQuestion.QuestionText = newQuestion.QuestionText;
                        existingQuestion.CorrectAnswer = newQuestion.CorrectAnswer;
                    }
                    else
                    {
                        existingQuiz.Questions.Add(new Question
                        {
                            QuestionText = newQuestion.QuestionText,
                            CorrectAnswer = newQuestion.CorrectAnswer,
                            QuizId = existingQuiz.Id
                        });
                    }
                }

                _unitOfWork.Quizs.Alter(existingQuiz);
                _unitOfWork.Commit();

                return RedirectToAction("Index");
            }

            ViewBag.lessons = _unitOfWork.Lessons.Get().ToList();
            return View(quiz);
        }



        public IActionResult Delete(int id)
        {
            var quiz = _unitOfWork.Quizs.GetOne(
                l => l.Id == id,
                includeProps: query => query.Include(l => l.Lesson).Include(e => e.Questions)
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
