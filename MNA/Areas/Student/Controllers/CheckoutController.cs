using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Stripe.Checkout;

namespace MNA.Areas.Student.Controllers
{
    [Area("student")]
    public class CheckoutController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public IActionResult Success(string session_id)
        {

            var service = new SessionService();
            var session = service.Get(session_id);

            if (session.PaymentStatus == "paid")
            {
                var applicationUserId = _userManager.GetUserId(User);

                // Get all items from the shopping cart
                var shoppingCarts = _unitOfWork.Carts.Get(
                    e => e.ApplicationUserId == applicationUserId
                ).ToList();

                // Remove items from the cart after successful payment
                foreach (var item in shoppingCarts)
                {
                    _unitOfWork.Carts.Delete(item);
                }

                _unitOfWork.Commit(); // Save changes

                TempData["success"] = "Payment completed successfully! Your courses have been removed from the cart.";
            }
            else
            {
                TempData["error"] = "Payment failed or was not completed.";
            }

            return View();
        }
        public IActionResult Cancel()
        {
            TempData["error"] = "Payment was canceled!";
            return View();
        }
    }
}
