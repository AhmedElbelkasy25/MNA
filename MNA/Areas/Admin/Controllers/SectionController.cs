using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MNA.Areas.Admin.Controllers
{
<<<<<<< HEAD
=======
    [Area("Admin")]
>>>>>>> ea3db1396122d650513b0827682b9c0b1110862c
    public class SectionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SectionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            ViewBag.Courses = _unitOfWork.Sections.Get().ToList();
            return View();
        }


        [HttpPost]
        public IActionResult Create(Section section)
        {
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
