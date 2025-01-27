using Microsoft.AspNetCore.Mvc;

namespace MNA.Areas.Admin.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
