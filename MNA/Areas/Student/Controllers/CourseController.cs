using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MNA.Areas.Student.Controllers
{
    [Area("Student")]
    public class CourseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
            {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }
        public IActionResult Index(int pageNumber = 1, int numOfItems = 9)
        {
            
            var courses = _unitOfWork.Courses.Get(
                includeProps: query => query.Include(c => c.Category).Include(c => c.Instructor)
            );

            int pages = (int)Math.Ceiling((double)courses.Count() / numOfItems);
            courses = courses.Skip((pageNumber - 1) * numOfItems).Take(numOfItems);
            CoursePaginationVM coursePaginationVM = new CoursePaginationVM()
            {
                Courses = courses.ToList(),
                Pages = pages,
                PageNumber = pageNumber
            };

            return View(model: coursePaginationVM);
        }

        public IActionResult Details(int Id)
        {
            var course = _unitOfWork.Courses.GetOne( filter:e  => e.Id == Id ,
                includeProps:e=>e.Include(e=>e.Instructor).Include(e=>e.Reviews).Include(e=>e.Sections).ThenInclude(e=>e.Lessons));
            var reviews = _unitOfWork.Reviews.Get(filter: e => e.CourseId == course.Id,
                includeProps: e => e.Include(e => e.Student).Include(e => e.Course));
            if (course == null)
            {
                return RedirectToAction("Index");
            }

            Course_ReviewsVM course_ReviewsVM = new Course_ReviewsVM()
            {
                Course = course,
                Reviews = reviews.ToList(),
            };

            return View(model: course_ReviewsVM);
        }
        public IActionResult ShowCourse(int Id)
        {
            var course = _unitOfWork.Courses.GetOne(filter: e => e.Id == Id,
                includeProps: e => e.Include(e => e.Instructor).Include(e => e.Reviews).Include(e => e.Sections).ThenInclude(e => e.Lessons)
                .ThenInclude(e => e.Quizs).ThenInclude(e=>e.Questions));
            var reviews = _unitOfWork.Reviews.Get(filter: e => e.CourseId == course.Id,
                includeProps: e => e.Include(e => e.Student).Include(e => e.Course));
            var stdNum = _unitOfWork.Enrollments.Get(filter: e => e.CourseId == Id).Count();
            var lsnNum = _unitOfWork.Lessons.Get(filter:e=>e.Section.CourseId == Id ,
                includeProps:e=>e.Include(e=>e.Section)).Count();

            Course_ReviewsVM course_ReviewsVM = new Course_ReviewsVM()
            {
                Course = course,
                Reviews = reviews.ToList(),
                StdNums = stdNum,
                LsnNums = lsnNum
            };

            return View(model: course_ReviewsVM);
        }

        public IActionResult GetQuizQuestions(int lessonId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var degrees = _unitOfWork.Degrees.Get(filter: e=>e.ApplicationUserId == userId);

            if (degrees.Any())
            {
                return RedirectToAction("GetDegree", new { stdId = userId });
            }




            var quizzes = _unitOfWork.Quizs.Get(filter: q => q.LessonId == lessonId,
                    includeProps: q => q.Include(q => q.Questions).Include(e=>e.Lesson).ThenInclude(e=>e.Section)).ToList();

            if (!quizzes.Any())
            {
                return NotFound("No quizzes found for this lesson.");
            }

            var questions = quizzes.SelectMany(q => q.Questions).ToList();

            return View(questions);
        }


        [HttpPost]
        public IActionResult SubmitQuiz(int lessonId, int marks , int totalMarks)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var lesson = _unitOfWork.Lessons.GetOne(filter:e => e.Id == lessonId );
            if (lesson == null)
            {
                return NotFound("Lesson not found.");
            }

            var degree = new Degree
            {
                ApplicationUserId = userId,
                CourseId = lesson.SectionId, // تأكد من أن الـ CourseId مرتبط بشكل صحيح
                LessonId = lessonId,
                Marks = marks,
                FinalDegree = totalMarks ,
                Date = DateOnly.FromDateTime(DateTime.Now)
            };

            _unitOfWork.Degrees.Create(degree);
            _unitOfWork.Commit();

            return Json(new { success = true, message = "Quiz submitted successfully!", score = marks });
        }

        public IActionResult GetDegree( string stdId)
        {
            var degrees = _unitOfWork.Degrees.Get(filter: e => e.ApplicationUserId == stdId , 
                includeProps:e=>e.Include(e=>e.Lesson).ThenInclude(e=>e.Quizs).ThenInclude(e=>e.Questions)).ToList();

            return View (model: degrees);


        }




        public IActionResult CreateReview(int CourseId)
        {
            
            ViewBag.Course = _unitOfWork.Courses.GetOne(e => e.Id == CourseId);
            return View();

        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult CreateReview(Review review)
        {
            ModelState.Remove("Course");
            ModelState.Remove("Student");
            if (ModelState.IsValid)
            {
                _unitOfWork.Reviews.Create(review);
                _unitOfWork.Commit();


                return RedirectToAction("ShowCourse", "Course", new { Id = review.CourseId });
            }

            return View(review);
        }


    }
}
