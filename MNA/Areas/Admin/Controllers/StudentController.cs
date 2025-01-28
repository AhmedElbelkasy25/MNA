using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;

namespace MNA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StudentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public StudentController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }
        public IActionResult Index(string? account)
        {
            if (account == null)
            {
                var allStudent = _unitOfWork.ApplicationUsers.Get();
                return View(allStudent.ToList());
            }
            var students = _unitOfWork.ApplicationUsers.Get().Where(e =>
                 e.Email.Contains(account) || e.UserName.Contains(account)).ToList();
            if (students.Any())
            {

                return View(model: students);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchForUser(string account)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", new { account = account });

            }
            return RedirectToAction("index");
        }

        public async Task<IActionResult> blockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                var result = await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(100));
                
                if (result.Succeeded)
                {
                    user.IsLocked = true;
                    await _userManager.UpdateAsync(user);
                    TempData["success"] = "The user has been blocked succefully";
                    var allUser = _userManager.Users.ToList();
                    return View("Index", allUser);
                }
                TempData["error"] = "The user has not been blocked";
                var allUser2 = _userManager.Users.ToList();
                return View("Index", allUser2);
            }
            return RedirectToAction("NotFoundPage","Home" , new {Area = "Student"});
        }

        public async Task<IActionResult> unBlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                
                var result = await _userManager.SetLockoutEndDateAsync(user, null);
                await _userManager.SetLockoutEnabledAsync(user, false);

                if (result.Succeeded)
                {
                    user.IsLocked = false;
                    await _userManager.UpdateAsync(user);
                    TempData["success"] = "The user is not blocked from thd site";
                    var allUser = _userManager.Users.ToList();
                    return View("Index", allUser);
                }
                TempData["error"] = "something error has happened";

                var allUser2 = _userManager.Users.ToList();
                return View("Index", allUser2);
            }
            return RedirectToAction("NotFoundPage","Home" , new {Area = "Student"});
        }

        [HttpGet]
        public async Task<IActionResult> AddToRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.ToList();
            var userToFrm = new AddToRoleVM()
            {
                Id = userId,
                Name = user.Name,
                Role = string.Join("", role),
                Roles = roles
            };
            

            return View(model: userToFrm);
        }
        [HttpPost]
        public async Task<IActionResult> AddToRole(AddToRoleVM userfrmVM)
        {

            var user = await _userManager.FindByIdAsync(userfrmVM.Id);
            if (user != null)
            {
                var oldRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, oldRoles);
                var result = await _userManager.AddToRoleAsync(user, userfrmVM.Role);
                if (result.Succeeded)
                {
                    TempData["success"] = "The User Role Has Been Changed";
                }
                return View("index", _userManager.Users.ToList());
            }
            return RedirectToAction("NotFoundPage","Home" , new {Area = "Student"});
        }



    }
}
