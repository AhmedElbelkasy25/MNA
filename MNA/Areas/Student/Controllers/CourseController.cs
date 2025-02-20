using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using System.Text.Json.Serialization;
using System.Text.Json;
using Stripe;
using Azure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Authorization;

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



        [AllowAnonymous]
        public IActionResult Index(int? categoryId, int pageNumber = 1, int numOfItems = 9)
        {
            
            var courses = _unitOfWork.Courses.Get(
                includeProps: query => query.Include(c => c.Category).Include(c => c.Instructor)
            );

            if (categoryId.HasValue)
            {
                courses = courses.Where(c => c.CategoryId == categoryId);
                ViewBag.CategoryName = _unitOfWork.Categories.GetOne(c => c.Id == categoryId).Name;
            }
            else
            {
                ViewBag.CategoryName = "All Courses";
            }

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

        [AllowAnonymous]
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




        [Authorize]
        [HttpPost]
        public IActionResult GetCoupon(string serial, int courseId)
        {
            if (serial==null || courseId == null)
            {
                return Json(new { Success = false, Message = "Invalid input." });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                //return Unauthorized();
                return Challenge();
            }


            var course = _unitOfWork.Courses.GetOne(e => e.Id == courseId);
            if (course == null)
            {
                return Json(new { Success = false, Message = "Course not found." });
            }

            var coupon = _unitOfWork.Coupons.GetOne(e => e.Serial == serial && e.CourseId == courseId);
            if (coupon == null)
            {
                return Json(new { Success = false, Message = "Coupon not found or invalid for this course." });
            }

            if (DateOnly.FromDateTime(DateTime.Now) > coupon.ExpireDate)
            {
                _unitOfWork.Coupons.Delete(coupon);
                _unitOfWork.Commit();
                return Json(new { Success = false, Message = "Coupon has expired." });
            }

            if (coupon.NumOfUsing <= 0)
            {
                _unitOfWork.Coupons.Delete(coupon);
                _unitOfWork.Commit();
                return Json(new { Success = false, Message = "Coupon has expired " });
            }

            var studentCoupon = _unitOfWork.StudentCoupons.GetOne(
                filter: e => e.ApplicationUserId == userId && e.CouponId == coupon.Id);

            if (studentCoupon != null)
            {
                return Json(new { Success = false, Message = "You have already used this coupon." });
            }

            
            double discountAmount = course.Price * (coupon.Discount);
            double discountedPrice = course.Price - discountAmount;

            _unitOfWork.StudentCoupons.Create(new StudentCouPons()
            {
                CouponId = coupon.Id,
                AmountOfDiscount = discountAmount,
                ApplicationUserId = userId,

            });
            _unitOfWork.Commit();
            return Json(new
            {
                Success = true,
                DiscountedPrice = discountedPrice,
                OriginalPrice = course.Price,
                Discount = Math.Round(discountAmount,1),
                CouponSerial = coupon.Serial
            });
        }



        [Authorize]
        public IActionResult ShowCourse(int Id)
        {
            var enroll = _unitOfWork.Enrollments.GetOne(filter: e => e.CourseId == Id,
                includeProps: e => e.Include(e => e.Course).ThenInclude(e => e.Reviews).Include(e => e.Course.Sections).ThenInclude(e => e.Lessons)
                .ThenInclude(e => e.Quizs).ThenInclude(e=>e.Questions));
            var reviews = _unitOfWork.Reviews.Get(filter: e => e.CourseId == enroll.Course.Id,
                includeProps: e => e.Include(e => e.Student).Include(e => e.Course));
            var stdNum = _unitOfWork.Enrollments.Get(filter: e => e.CourseId == Id).Count();
            var lsnNum = _unitOfWork.Lessons.Get(filter:e=>e.Section.CourseId == Id ,
                includeProps:e=>e.Include(e=>e.Section)).Count();

            Course_ReviewsVM course_ReviewsVM = new Course_ReviewsVM()
            {
                Course = enroll.Course,
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

            var degrees = _unitOfWork.Degrees.Get(filter: e=>e.ApplicationUserId == userId && e.LessonId == lessonId);

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
                CourseId = lesson.SectionId, 
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
        public IActionResult CreateReview(Models.Review review)
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
