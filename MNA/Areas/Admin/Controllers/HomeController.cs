using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using System.Xml.Schema;

namespace MNA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IUnitOfWork unitOfWork ,
            UserManager<ApplicationUser> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }
        public IActionResult Index()
        {
            var courses = _unitOfWork.Courses.Get().Count();
            var users = _unitOfWork.ApplicationUsers.Get().Count();
            var revenue = _unitOfWork.Payments.Get().Select(e=>e.Amount).Sum();
            var trans = _unitOfWork.Payments.Get(includeProps:e=>e.Include(e=>e.Course)
            .Include(e=>e.Student)).OrderByDescending(e => e.Id).ToList();

            var AdminPage = new AdminHome_User_course_money_payments_VM()
            {
                courses = courses,
                users = users,
                revenue = revenue,
                trans = trans
            };

            
            return View(model: AdminPage);
        }
    }
}
