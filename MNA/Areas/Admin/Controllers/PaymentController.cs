using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MNA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }
        public IActionResult Index()
        {
            var payments = _unitOfWork.Payments.Get(includeProps: e => e.Include(e => e.Course).Include(e => e.Student)).ToList();
            return View(payments);
        }
        public IActionResult Create()
        {
            ViewBag.Students = _unitOfWork.ApplicationUsers.Get().ToList();
            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();

            return View();
        }
        [HttpPost]
        public IActionResult Create(Payment payment)
        {
            ModelState.Remove("Student");
            ModelState.Remove("Course");
            if (ModelState.IsValid)
            {
                _unitOfWork.Payments.Create(payment);
                _unitOfWork.Commit();
                TempData["Success"] = "Data Added Successfully";
                return RedirectToAction("Index");
            }
            return View(payment);
        }

        public IActionResult Edit(int Id)
        {
            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();
            ViewBag.Students = _unitOfWork.ApplicationUsers.Get().ToList();
            var payment = _unitOfWork.Payments.GetOne(filter: x => x.Id == Id);
            return View(payment);
        }
        [HttpPost]
        public IActionResult Edit(Payment payment)
        {
            if (!ModelState.IsValid)
            {
                
                _unitOfWork.Payments.Alter(payment);
                _unitOfWork.Commit();
                TempData["Success"] = "Payment Edited Successfully";

                return RedirectToAction("Index");

            }
            return View(payment);
        }

        public IActionResult Delete(int Id)
        {
            var payment = _unitOfWork.Payments.GetOne(filter: x => x.Id == Id);
            _unitOfWork.Payments.Delete(payment);
            _unitOfWork.Commit();
            TempData["success"] = "Payment deleted successfuly";
            return RedirectToAction("Index");
        }

    }
}
