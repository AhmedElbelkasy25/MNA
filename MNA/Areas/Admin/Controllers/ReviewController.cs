using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MNA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }
        public IActionResult Index()
        {
            var reviews = _unitOfWork.Reviews.Get(includeProps:e=>e.Include(e=>e.Student)
            .Include(e=>e.Course)).ToList();
            
            return View(model:reviews);
        }

        public IActionResult Create() 
        {
            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();
            return View();
        
        }
        [HttpPost]
        public IActionResult Create(Review review)
        {
            ModelState.Remove("Course");
            ModelState.Remove("Student");
            if (ModelState.IsValid)
            {
                _unitOfWork.Reviews.Create(review);
                _unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            return View(review);
        }


        public IActionResult Edit(int Id)
        {
            var review = _unitOfWork.Reviews.GetOne(e=>e.Id== Id);
            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();
            return View(model:review);
        }

        [HttpPost]
        public IActionResult Edit(Review review)
        {
            if (!ModelState.IsValid)
            {
                _unitOfWork.Reviews.Alter(review);
                _unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            return View(review);
        }

        public IActionResult Delete(int Id)
        {
            var review = _unitOfWork.Reviews.GetOne(e => e.Id == Id);
            _unitOfWork.Reviews.Delete(review);
            _unitOfWork.Commit();
            return RedirectToAction("Index");
        }

    }

    


}
