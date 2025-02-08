using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DataAccess.Repository.IRepository;
using Models;
using Stripe;

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
            var service = new Stripe.Checkout.SessionService();
            var session = service.Get(session_id);

            if (session.PaymentStatus == "paid")
            {
                var applicationUserId = _userManager.GetUserId(User);

                // Get all items from the shopping cart
                var shoppingCarts = _unitOfWork.Carts.Get(e => e.ApplicationUserId == applicationUserId).ToList();

                foreach (var item in shoppingCarts)
                {
                    // Add courses to enrollment
                    var enrollment = new Enrollment
                    {
                        ApplicationUserId = applicationUserId,
                        CourseId = item.CourseId,
                        ExpireDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(6)) // Example expiration
                    };

                    _unitOfWork.Enrollments.Create(enrollment);
                    _unitOfWork.Carts.Delete(item);
                }

                _unitOfWork.Commit(); // Save changes
                TempData["success"] = "Payment completed successfully! Your courses have been added.";
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

        [HttpPost]
        public IActionResult Refund(string paymentIntentId)
        {
            try
            {
                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                    Reason = "requested_by_customer"
                };

                var refundService = new RefundService();
                var refund = refundService.Create(refundOptions);

                if (refund.Status == "succeeded")
                {
                    TempData["success"] = "Refund processed successfully!";
                }
                else
                {
                    TempData["error"] = "Refund failed!";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Refund error: {ex.Message}";
            }

            return RedirectToAction("Index", "Course");
        }
    }
}
