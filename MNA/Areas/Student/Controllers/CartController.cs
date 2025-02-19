using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Stripe;
using Stripe.Checkout;

namespace MNA.Areas.Student.Controllers
{
    [Area("student")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var applicationUserId = _userManager.GetUserId(User);
            var shoppingCarts = _unitOfWork.Carts.Get(
                e => e.ApplicationUserId == applicationUserId,
                includeProps: q => q.Include(c => c.Course) 
            ).ToList();

            ViewBag.TotalPrice = 0;//shoppingCarts.Sum(e => e.Course.Price * e.Count);

            return View(shoppingCarts);
        }


        public IActionResult AddToCart(int courseId, int count = 1)
        {
            var applicationUserId = _userManager.GetUserId(User);
            double newPrice = 0;
            if (applicationUserId == null)
            {
                TempData["error"] = "You must be logged in to enroll in a course.";
                return RedirectToAction("Index", "Home");
            }
            var course = _unitOfWork.Courses.GetOne(e=>e.Id == courseId);
            //var cartItem = _unitOfWork.Carts.GetOne(e => e.ApplicationUserId == applicationUserId && e.CourseId == courseId);
            var stdCpn = _unitOfWork.StudentCoupons.GetOne(filter: e => e.Coupon.CourseId == courseId && e.ApplicationUserId == applicationUserId
            , includeProps: e=>e.Include(e=>e.Coupon));
            if(stdCpn!= null)
            {
                if (stdCpn.Coupon.NumOfUsing <= 0 || stdCpn.Coupon.ExpireDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    _unitOfWork.StudentCoupons.Delete(stdCpn);
                    var cpn = _unitOfWork.Coupons.GetOne(e => e.Id == stdCpn.CouponId);
                    _unitOfWork.Coupons.Delete(cpn);
                    _unitOfWork.Commit();
                    stdCpn = _unitOfWork.StudentCoupons.GetOne(filter: e => e.Coupon.CourseId == courseId && e.ApplicationUserId == applicationUserId);


                }
            }
            
            if (stdCpn != null)
            {
                newPrice = (double)(course.Price - stdCpn.AmountOfDiscount);

            }
            //if (cartItem != null)
            //{
            //    if ( newPrice != 0)
            //    {
            //        cartItem.DiscountedPrice = newPrice;

            //    }
            //    cartItem.Count += count;
            //    _unitOfWork.Carts.Alter(cartItem);
            //}
            
                
                var shoppingCart = new Cart
                {
                    CourseId = courseId,
                    Count = count, 
                    DiscountedPrice = newPrice,
                    ApplicationUserId = applicationUserId
                };
                _unitOfWork.Carts.Create(shoppingCart);
            

            _unitOfWork.Commit();

            TempData["success"] = "Course added to cart successfully!";

            return RedirectToAction("Index");
        }


        public IActionResult Increment(int cartId)
        {
            var cartItem = _unitOfWork.Carts.GetOne(e => e.Id == cartId);

            if (cartItem != null)
            {
                cartItem.Count++;
                _unitOfWork.Carts.Alter(cartItem);
                _unitOfWork.Commit();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Decrement(int cartId)
        {
            var cartItem = _unitOfWork.Carts.GetOne(e => e.Id == cartId);

            if (cartItem != null)
            {
                cartItem.Count--;
                if (cartItem.Count <= 0)
                    cartItem.Count = 1;

                _unitOfWork.Carts.Alter(cartItem);
                _unitOfWork.Commit();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Remove(int courseId)
        {
            var applicationUserId = _userManager.GetUserId(User);
            var cartItem = _unitOfWork.Carts.GetOne(e => e.ApplicationUserId == applicationUserId && e.CourseId == courseId);

            if (cartItem != null)
            {
                _unitOfWork.Carts.Delete(cartItem);
                _unitOfWork.Commit();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Pay(Cart cart)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/student/checkout/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/student/checkout/cancel",

            };

            var applicationUserId = _userManager.GetUserId(User);
            var shoppingCarts = _unitOfWork.Carts.Get(
               e => e.ApplicationUserId == applicationUserId,
               includeProps: q => q.Include(c => c.Course)
                ).ToList();


            foreach (var item in shoppingCarts)
            {
                double coursePrice;
                if(item.DiscountedPrice != null)
                {
                    coursePrice = Math.Round((double) item.DiscountedPrice ,2);
                }
                else { coursePrice = (double)item.Course.Price; }
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Course.Title,
                            Description = item.Course.Description,
                        },
                        
                        UnitAmount = (long)coursePrice * 100,
                    },
                    Quantity = item.Count,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }


        //public IActionResult Pay(Cart cart)
        //{
        //    var options = new SessionCreateOptions
        //    {
        //        PaymentMethodTypes = new List<string> { "card" },
        //        LineItems = new List<SessionLineItemOptions>(),
        //        Mode = "payment",
        //        SuccessUrl = $"{Request.Scheme}://{Request.Host}/student/checkout/success?session_id={{CHECKOUT_SESSION_ID}}",
        //        CancelUrl = $"{Request.Scheme}://{Request.Host}/student/checkout/cancel",
        //    };

        //    var applicationUserId = _userManager.GetUserId(User);
        //    var shoppingCarts = _unitOfWork.Carts.GetAll(
        //        filter: e => e.ApplicationUserId == applicationUserId,
        //        includeProps: q => q.Include(c => c.Course)
        //    ).ToList();

        //    foreach (var item in shoppingCarts)
        //    {
        //        // Check if a coupon is applied for this course
        //        var studentCoupon = _unitOfWork.StudentCoupons.GetFirstOrDefault(
        //            filter: e => e.ApplicationUserId == applicationUserId && e.Coupon.CourseId == item.Course.Id);

        //        double price = item.Course.Price;

        //        if (studentCoupon != null)
        //        {
        //            // Apply the coupon discount
        //            var coupon = studentCoupon.Coupon;
        //            double discountAmount = price * (coupon.Discount / 100);
        //            price -= discountAmount;

        //            // Decrease the number of uses for the coupon
        //            coupon.NumOfUsing--;
        //            _unitOfWork.Coupons.Update(coupon);
        //            _unitOfWork.Save();
        //        }

        //        options.LineItems.Add(new SessionLineItemOptions
        //        {
        //            PriceData = new SessionLineItemPriceDataOptions
        //            {
        //                Currency = "egp",
        //                ProductData = new SessionLineItemPriceDataProductDataOptions
        //                {
        //                    Name = item.Course.Title,
        //                    Description = item.Course.Description,
        //                },
        //                UnitAmount = (long)(price * 100), // Convert to cents
        //            },
        //            Quantity = item.Count,
        //        });
        //    }

        //    var service = new SessionService();
        //    var session = service.Create(options);
        //    return Redirect(session.Url);
        //}




        [HttpGet]
        public IActionResult GetCartCount()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(0);
            }

            int cartCount = _unitOfWork.Carts.Get(c => c.ApplicationUserId == userId).Sum(c => c.Count);
            return Json(cartCount);
        }
        [HttpPost]
        public IActionResult AddToFavorites(int courseId)
        {
            var applicationUserId = _userManager.GetUserId(User);

            if (applicationUserId == null)
            {
                return Json(new { success = false, message = "You must be logged in!" });
            }

            var existingFavorite = _unitOfWork.Favourites.GetOne(f => f.ApplicationUserId == applicationUserId && f.CourseId == courseId);

            if (existingFavorite == null)
            {
                var favorite = new Favourite
                {
                    ApplicationUserId = applicationUserId,
                    CourseId = courseId
                };

                _unitOfWork.Favourites.Create(favorite);
                _unitOfWork.Commit();

                return Json(new { success = true, message = "Added to favorites!" });
            }

            return Json(new { success = false, message = "Already in favorites!" });
        }

    }
}
