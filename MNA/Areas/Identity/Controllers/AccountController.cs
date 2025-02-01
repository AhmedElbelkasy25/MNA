using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MNA.Utility;
using Models;
using Models.ViewModels;

namespace MNA.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager ,
            SignInManager<ApplicationUser> signInManager ,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IEmailSender emailSender
            )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._mapper = mapper;
            this._emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Register()
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new("Admin"));
                await _roleManager.CreateAsync(new("Instructor"));
                await _roleManager.CreateAsync(new("student"));
            }

            //await _userManager.AddToRoleAsync(user, "Admin");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegiesterVm userLog /*, IFormFile? file*/)
        {
           
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    UserName = userLog.UserName,
                    Email = userLog.Email,
                    
                };
                var usrTest= await _userManager.FindByEmailAsync(user.Email);
                if (usrTest != null)
                {
                    ModelState.AddModelError("Email", "this account is already exist");
                    return View(userLog);
                }

                var result = await _userManager.CreateAsync(user, userLog.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home" , new { area = "student" } );
                }
                else
                {
                    ModelState.AddModelError("Password", "Password must be has a Capital and Small letters and numbers");
                }



            }

            return View(userLog);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM log)
        {
            if (ModelState.IsValid)
            {
                var appUserWithEmail = await _userManager.FindByEmailAsync(log.Account);
                var appUserWithUserName = await _userManager.FindByNameAsync(log.Account);
                var appUser = appUserWithEmail ?? appUserWithUserName;
                if (appUser != null)
                {
                    if (await _userManager.IsLockedOutAsync(appUser))
                    {
                        ModelState.AddModelError(string.Empty, "This account has been blocked.");
                        return View(log);
                    }
                    //var result = await _userManager.CheckPasswordAsync(appUserWithEmail ?? appUserWithUserName , log.Password);
                    var result = await _userManager.CheckPasswordAsync(appUser, log.Password);
                    if (result)
                    {
                        await _signInManager.SignInAsync(appUser,
                            log.RememberMe);
                        return RedirectToAction("Index", "Home", new { area = "student" });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Your account or Password is not correct");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Your account or Password is not correct");
                }

            }
            return View(log);
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(EmailVM userEmail)
        {
            if (ModelState.IsValid)
            {
                var appUserithEmail = await _userManager.FindByEmailAsync(userEmail.Account);
                if (appUserithEmail != null)
                {
                    // Create a link for changing the password
                    var changePasswordLink = Url.Action("ChangeForgottenPassword", "Account", new { acc = userEmail.Account }, Request.Scheme);
                    var emailBody = $"<p>Click the link below to change your password:</p><p><a href='{changePasswordLink}'>Change Your Password</a></p>";

                    await _emailSender.SendEmailAsync(userEmail.Account, "Change Your Password", emailBody);

                    return RedirectToAction("Index", "home", new { area = "student" });
                }
                else
                {
                    ModelState.AddModelError("Account", "This Account Doesn't Exist");
                }
            }

            return View(userEmail);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeForgottenPassword(string acc)
        {
            //var appUserithEmail = await _userManager.FindByEmailAsync(acc);
            //var appUserithName = await _userManager.FindByNameAsync(acc);
            var account = new ForgetPassordVM()
            {
                Account = acc
            };
            return View(model: account);

            
        }

        [HttpPost]
        public async Task<IActionResult> ChangeForgottenPassword(ForgetPassordVM newpass)
        {
            if (ModelState.IsValid)
            {
                var appUserithEmail = await _userManager.FindByEmailAsync(newpass.Account);
                //var appUserithName = await _userManager.FindByNameAsync(newpass.Account);

                await _userManager.RemovePasswordAsync(appUserithEmail);
                var result = await _userManager.AddPasswordAsync(appUserithEmail, newpass.Password);
                if (result.Succeeded)
                {
                    TempData["success"] = "Password has been changed successfully";
                    return RedirectToAction("Index", "Home", new { Area = "Student" });
                }
            }
            return View(newpass);
        }

        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM newpass)
        {
            if (ModelState.IsValid)
            {
                var appUser = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

                var result = await _userManager.ChangePasswordAsync(appUser, newpass.OldPassword, newpass.NewPassword);
                if (result.Succeeded)
                {
                    TempData["success"] = "The password has changed correctlly";
                    return RedirectToAction("Index", "Home");

                }
                ModelState.AddModelError(string.Empty, "there is some thing wrong ");
            }
            return View(newpass);
        }

        public async Task<IActionResult> GetProfile(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            return View(model: user);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(model: user);
            }
            return RedirectToAction("NotFoundPage", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            ModelState.Remove("ImgUrl");

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var oldUser = await _userManager.FindByIdAsync(userId);

                oldUser.Address = user.Address;
                oldUser.Name = user.Name;
                oldUser.UserName = user.UserName;
                oldUser.PhoneNumber = user.PhoneNumber;
                oldUser.Email = user.Email;
                await _userManager.UpdateAsync(oldUser);
                TempData["success"] = "Edit User successfully";
                return RedirectToAction("GetProfile", "Account", new { name = User.Identity.Name });
            }
            return View(user);
        }
        public IActionResult ChangePhoto()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePhoto(IFormFile? file)
        {
            var userId = _userManager.GetUserId(User);
            var oldUser = await _userManager.FindByIdAsync(userId);


            if (file != null && file.Length > 0)
            {
                // Genereate name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Save in wwwroot
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\AdminImg", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                //delete old img
                if (oldUser.ImgUrl != null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\AdminImg", oldUser.ImgUrl);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Save in db
                oldUser.ImgUrl = fileName;
            }

            await _userManager.UpdateAsync(oldUser);
            TempData["success"] = "Edit Photo successfully";
            return RedirectToAction("GetProfile", "Account", new { name = User.Identity.Name });
        }





        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
