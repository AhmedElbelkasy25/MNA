using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using System.Threading.Tasks;

namespace MNA.Areas.Student.Controllers
{
    [Area("Student")]
    public class InstructorApplicationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorApplicationController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Apply()
        {
            var user = await _userManager.GetUserAsync(User);
            var existingApplication = _unitOfWork.InstructorApplications.GetOne(a => a.UserId == user.Id);

            if (existingApplication != null)
            {
                ViewBag.Message = "You have already applied.";
                return View("ApplicationStatus");
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Apply(InstructorApplication model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
                return View(model);

            var newApplication = new InstructorApplication
            {
                UserId = user.Id,
                FullName = model.FullName,
                Email = model.Email,
                Bio = model.Bio
            };

            _unitOfWork.InstructorApplications.Create(newApplication);
            _unitOfWork.Commit();

            ViewBag.Message = "Your application has been submitted.";
            return View("ApplicationStatus");
        }
    }
}
