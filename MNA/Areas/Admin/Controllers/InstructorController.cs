using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MNA.Areas.Admin.Controllers
{

    [Area("Admin")]

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

        // GET: Instructor
        public IActionResult Index()
        {
            // Use Include to load related courses
            var instructors = _unitOfWork.Instructors.Get(
                includeProps: query => query.Include(i => i.Courses)
            ).ToList();

            return View(instructors);
        }

        // GET: Instructor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Instructor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Instructors.Create(instructor);
                _unitOfWork.Commit();
                return RedirectToAction(nameof(Index));
            }

            return View(instructor);
        }

        // GET: Instructor/Edit/5
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

        // POST: Instructor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Instructor instructor)
        {
            if (id != instructor.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Instructors.Alter(instructor);
                _unitOfWork.Commit();
                return RedirectToAction(nameof(Index));
            }

            return View(instructor);
        }

        // GET: Instructor/Delete/5
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

        // POST: Instructor/Delete/5
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
            return RedirectToAction(nameof(Index));
        }
    }
}
