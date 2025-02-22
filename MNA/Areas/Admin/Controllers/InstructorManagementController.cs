using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using System.Threading.Tasks;

namespace MNA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class InstructorManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorManagementController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var applications = _unitOfWork.InstructorApplications.Get();
            return View(applications);
        }

        public async Task<IActionResult> Approve(int id)
        {
            var application = _unitOfWork.InstructorApplications.GetOne(a => a.Id == id);
            if (application == null) return NotFound();

            var user = await _userManager.FindByIdAsync(application.UserId);
            if (user == null) return NotFound();

            await _userManager.AddToRoleAsync(user, "Instructor");
            application.IsApproved = true;

            _unitOfWork.InstructorApplications.Alter(application);
            _unitOfWork.Commit();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Reject(int id)
        {
            var application = _unitOfWork.InstructorApplications.GetOne(a => a.Id == id);
            if (application != null)
            {
                _unitOfWork.InstructorApplications.Delete(application);
                _unitOfWork.Commit();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
