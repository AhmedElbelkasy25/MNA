using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
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

            ViewBag.TotalPrice = shoppingCarts.Sum(e => e.Course.Price * e.Count);

            return View(shoppingCarts);
        }


        public IActionResult AddToCart(int courseId, int count = 1)
        {
            var applicationUserId = _userManager.GetUserId(User);

            if (applicationUserId == null)
            {
                TempData["error"] = "You must be logged in to enroll in a course.";
                return RedirectToAction("Index", "Home");
            }

            var cartItem = _unitOfWork.Carts.GetOne(e => e.ApplicationUserId == applicationUserId && e.CourseId == courseId);

            if (cartItem != null)
            {
                cartItem.Count += count; // Ensure the count is updated
                _unitOfWork.Carts.Alter(cartItem);
            }
            else
            {
                var shoppingCart = new Cart
                {
                    CourseId = courseId,
                    Count = count, // ✅ Ensure Count is at least 1
                    ApplicationUserId = applicationUserId
                };
                _unitOfWork.Carts.Create(shoppingCart);
            }

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
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
            };

            var applicationUserId = _userManager.GetUserId(User);
            var shoppingCarts = _unitOfWork.Carts.Get(
       e => e.ApplicationUserId == applicationUserId,
       includeProps: q => q.Include(c => c.Course)
        ).ToList();


            foreach (var item in shoppingCarts)
            {
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
                        UnitAmount = (long)item.Course.Price * 100,
                    },
                    Quantity = item.Count,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }
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
