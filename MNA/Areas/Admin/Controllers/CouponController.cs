using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;

namespace MNA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CouponController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }
        public IActionResult Index(string? CourseName)
        {
            IEnumerable<Coupon> coupons;
            if (CourseName == null)
            {
                 coupons = _unitOfWork.Coupons.Get(includeProps: e => e.Include(e => e.Course))
                    .OrderByDescending(e => e.Id).ToList();
            }
            else
            {
                 coupons = _unitOfWork.Coupons.Get(includeProps: e => e.Include(e => e.Course),
                    filter: e => e.Course.Title.Contains(CourseName)).ToList();
            }
            if (coupons != null)
            {
                return View(model: coupons);
            }
            return RedirectToAction("NotFoundPage");
        }
        [HttpPost]
        public IActionResult SearchForCourse(string CourseName)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", new { CourseName = CourseName });

            }
            return RedirectToAction("index");
        }

        public IActionResult Create()
        {
            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();
           
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreatingCouponVM crtCoupon)
        {
            if (ModelState.IsValid)
            {
                if (crtCoupon.Discount > 1)
                    crtCoupon.Discount /= 100;

                if (crtCoupon.Serial == null)
                {
                    crtCoupon.Serial = Guid.NewGuid().ToString().Substring(0, 16);
                }

                        _unitOfWork.Coupons.Create(new Coupon()
                        {
                            ExpireDate = crtCoupon.ExpireDate,
                            Discount = crtCoupon.Discount,
                            Serial = crtCoupon.Serial,
                            CourseId = crtCoupon.CourseId

                        });
                        _unitOfWork.Commit();

                
                TempData["success"] = "coupon Added successfuly";
                return RedirectToAction("Index");

            }
                return View(crtCoupon);
        }

        public IActionResult Edit(int Id)
        {
            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();
            var Coupon = _unitOfWork.Coupons.GetOne(filter: x => x.Id == Id);
            return View(Coupon);
        }
        [HttpPost]
        public IActionResult Edit(Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                if (coupon.Discount > 1)
                    coupon.Discount /= 100;

                _unitOfWork.Coupons.Alter(coupon);
                _unitOfWork.Commit();
                TempData["Success"] = "Coupon Edited Successfully";

                return RedirectToAction("Index");

            }
        return View(coupon);
        }


        public IActionResult Delete(int Id)
        {
            var Coupon = _unitOfWork.Coupons.GetOne(filter: x => x.Id == Id);
            _unitOfWork.Coupons.Delete(Coupon);
            _unitOfWork.Commit();
            TempData["success"] = "coupon deleted successfuly";
            return RedirectToAction("Index");
        }

    }
}
