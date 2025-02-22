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


            //var existingApplication = _unitOfWork.InstructorApplications
            //    .Get(filter: a => a.UserId == user.Id).FirstOrDefault();





            //if (existingApplication != null)
            //{
            //    ViewBag.Message = "You have already applied.";
            //    return View("ApplicationStatus");
            //}

            // Populate the view model with current user data
            var model = new InstructorApplication
            {
                Email = user.Email
            };

            return View(model);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Apply(InstructorApplication model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                return View(model); // Return with validation errors
            }

            //var newApplication = new InstructorApplication
            //{
            //    UserId = user.Id,
            //    FullName = model.FullName,
            //    Email = model.Email,
            //    Bio = model.Bio
            //};

            _unitOfWork.InstructorApplications.Create(new InstructorApplication
            {
                UserId = user.Id,
                FullName = model.FullName,
                Email = model.Email,
                Bio = model.Bio
            });
            _unitOfWork.Commit();

            ViewBag.Message = "Your application has been submitted.";
            return View("ApplicationStatus");
        }

    }
}
