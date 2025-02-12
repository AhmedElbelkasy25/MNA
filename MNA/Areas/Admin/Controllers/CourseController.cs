using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;

namespace MNA.Areas.Admin.Controllers
{

    [Area("Admin")]

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

        // GET: Course
        public IActionResult Index(int pageNumber = 1, int numOfItems = 9)
        {

            var courses = _unitOfWork.Courses.Get(
                includeProps: query => query.Include(c => c.Category).Include(c => c.Instructor).
                Include(e => e.Sections).ThenInclude(e => e.Lessons).ThenInclude(e => e.Quizs)
                .ThenInclude(e => e.Questions)
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

        // GET: Course/Create
        public IActionResult Create()
        {
            // Pass related Category and Instructor data for dropdowns
            ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
            ViewBag.Instructors = _unitOfWork.Instructors.Get().ToList();
            return View();
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course, IFormFile? file)
        {
            ModelState.Remove("ImgUrl");
            ModelState.Remove("Category");
            ModelState.Remove("Instructor");
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Genereate name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Save in wwwroot
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Course", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }

                    // Save in db
                    course.ImgUrl = fileName;
                }
                else
                {
                    ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
                    ViewBag.Instructors = _unitOfWork.Instructors.Get().ToList();
                    return View(course);

                }
                _unitOfWork.Courses.Create(course);
                _unitOfWork.Commit();
                TempData["success"] = "Course has been Added Successfully";
                return RedirectToAction(nameof(Index));
            }

            
            ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
            ViewBag.Instructors = _unitOfWork.Instructors.Get().ToList();
            return View(course);
        }

        
        public IActionResult Edit(int id)
        {
            var course = _unitOfWork.Courses.GetOne(
                c => c.Id == id,
                includeProps: query => query.Include(c => c.Category).Include(c => c.Instructor)
            );

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
            ViewBag.Instructors = _unitOfWork.Instructors.Get().ToList();

            return View(course);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Course course , IFormFile? file)
        {
            if (id != course.Id)
            {
                return BadRequest();
            }
            ModelState.Remove("ImgUrl");
            ModelState.Remove("Category");
            ModelState.Remove("Instructor");

            var oldCourse = _unitOfWork.Courses.GetOne(e => e.Id == course.Id, tracked: false);
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Genereate name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Save in wwwroot
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Course", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    //delete old img
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Course", oldCourse.ImgUrl);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                    // Save in db
                    course.ImgUrl = fileName;
                }
                else
                {
                    course.ImgUrl = oldCourse.ImgUrl;
                }
                // disable the trigger of courses table
                _unitOfWork.ExecuteRawSql("DISABLE TRIGGER triggerUpdateInstructorRating ON Courses");


                _unitOfWork.Courses.Alter(course);
                _unitOfWork.Commit();
                // enable the trigger of courses table
                _unitOfWork.ExecuteRawSql("ENABLE TRIGGER triggerUpdateInstructorRating ON Courses");
                TempData["success"] = "Course has been Edited Successfully";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
            ViewBag.Instructors = _unitOfWork.Instructors.Get().ToList();
            return View(course);
        }

        
        public IActionResult Delete(int id)
        {
            var course = _unitOfWork.Courses.GetOne(
                c => c.Id == id,
                includeProps: query => query.Include(c => c.Category).Include(c => c.Instructor)
            );

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var course = _unitOfWork.Courses.GetOne(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            _unitOfWork.Courses.Delete(course);
            _unitOfWork.Commit();

            TempData["success"] = "the Course has been Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
