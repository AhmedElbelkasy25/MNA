using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;

namespace MNA.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class InstructorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }


        public IActionResult Index()
        {

            var instructors = _unitOfWork.Instructors.Get(
                includeProps: query => query.Include(i => i.Courses)
            ).ToList();

            return View(instructors);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Models.Instructor instructor, IFormFile? file)
        {
            ModelState.Remove("PicUrl");
            ModelState.Remove("Rating");
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Genereate name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Save in wwwroot
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Instructors", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }

                    // Save in db
                    instructor.PicUrl = fileName;
                }

                _unitOfWork.Instructors.Create(instructor);
                _unitOfWork.Commit();
                TempData["success"] = "the Instructor has been Added successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(instructor);
        }


        public IActionResult Edit(int id)
        {
            var instructor = _unitOfWork.Instructors.GetOne(
                i => i.Id == id,
                includeProps: query => query.Include(i => i.Courses)
            );

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Models.Instructor instructor, IFormFile file)
        {
            if (id != instructor.Id)
            {
                return View(instructor);
            }
            ModelState.Remove("file");
            ModelState.Remove("Rating");
            var oldInstructor = _unitOfWork.Instructors.GetOne(filter: e => e.Id == instructor.Id, tracked: false);
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Genereate name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Save in wwwroot
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\instructors", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    //delete old img
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\instructors", oldInstructor.PicUrl);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                    // Save in db
                    instructor.PicUrl = fileName;
                }
                else
                {
                    instructor.PicUrl = oldInstructor.PicUrl;
                }


                _unitOfWork.Instructors.Alter(instructor);
                _unitOfWork.Commit();
                TempData["success"] = "the Instructor has been Edited successfully";
                return RedirectToAction("Index");
            }

            return View(instructor);
        }


        public IActionResult GetProfile(int Id)
        {
            if (Id == null)
            {
                RedirectToAction("NotFoundPage", "Home");
            }
            var instructor = _unitOfWork.Instructors.GetOne(e => e.Id == Id,
                includeProps: e => e.Include(e => e.Courses));
            return View(instructor);
        }

        public IActionResult Delete(int id)
        {
            var instructor = _unitOfWork.Instructors.GetOne(
                i => i.Id == id,
                includeProps: query => query.Include(i => i.Courses)
            );

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var instructor = _unitOfWork.Instructors.GetOne(i => i.Id == id);

            if (instructor == null)
            {
                return NotFound();
            }

            _unitOfWork.Instructors.Delete(instructor);
            _unitOfWork.Commit();
            TempData["success"] = "the Instructor has been Deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}