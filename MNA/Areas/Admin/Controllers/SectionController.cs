using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MNA.Areas.Admin.Controllers
{

    [Area("Admin")]

    public class SectionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SectionController(IUnitOfWork unitOfWork , RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            this._roleManager = roleManager;
        }

        public IActionResult Index()
        {

            var sections = _unitOfWork.Sections.Get(
                includeProps: query => query.Include(l => l.Course)
            ).ToList();

            return View(sections);
        }

        public IActionResult Create()
        {

            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();
            //ViewBag.Courses = _unitOfWork.Sections.Get(includeProps:e=>e.Include(e=>e.Course)
            //.ThenInclude(e=>e.Instructor).ThenInclude(e=>e.),filter: e=>e.Course.Instructor. );
            return View();
        }


        [HttpPost]
        public IActionResult Create(Section section)
        {
            ModelState.Remove("Course");
            ModelState.Remove("Lessons");
            if (ModelState.IsValid)
            {
                _unitOfWork.Sections.Create(section);
                _unitOfWork.Commit();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();
            return View(section);
        }

        public IActionResult Edit(int id)
        {
            var section = _unitOfWork.Sections.GetOne(
                l => l.Id == id,
                includeProps: query => query.Include(l => l.Course)
            );

            if (section == null)
            {
                return NotFound();
            }
            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();
            return View(section);
        }

        [HttpPost]
        public IActionResult Edit(int id, Section section)
        {
            if (id != section.Id)
            {
                return BadRequest();
            }
            ModelState.Remove("Course");
            ModelState.Remove("Lessons");
            if (ModelState.IsValid)
            {
                _unitOfWork.Sections.Alter(section);
                _unitOfWork.Commit();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Courses = _unitOfWork.Courses.Get().ToList();
            return View(section);
        }

        public IActionResult Delete(int id)
        {
            var section = _unitOfWork.Sections.GetOne(
                l => l.Id == id,
                includeProps: query => query.Include(l => l.Course)
            );

            if (section == null)
            {
                return NotFound();
            }

            return View(section);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var section = _unitOfWork.Sections.GetOne(l => l.Id == id);

            if (section == null)
            {
                return NotFound();
            }

            _unitOfWork.Sections.Delete(section);
            _unitOfWork.Commit();
            return RedirectToAction(nameof(Index));
        }
    }
}
