﻿using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MNA.Areas.Admin.Controllers
{

    [Area("Admin")]

    public class CourseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
        }

        // GET: Course
        public IActionResult Index()
        {
            // Include related Category and Instructor data
            var courses = _unitOfWork.Courses.Get(
                includeProps: query => query.Include(c => c.Category).Include(c => c.Instructor)
            ).ToList();

            return View(courses);
        }

        // GET: Course/Create
        public IActionResult Create()
        {
            // Pass related Category and Instructor data for dropdowns
            ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
            ViewBag.Instructors = _unitOfWork.Instructors.Get().ToList();
            return View();
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course, IFormFile? file)
        {
            ModelState.Remove("ImgUrl");
            ModelState.Remove("Category");
            ModelState.Remove("Instructor");
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Genereate name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Save in wwwroot
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Course", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }

                    // Save in db
                    course.ImgUrl = fileName;
                }
                else
                {
                    ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
                    ViewBag.Instructors = _unitOfWork.Instructors.Get().ToList();
                    return View(course);

                }
                _unitOfWork.Courses.Create(course);
                _unitOfWork.Commit();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate ViewBag if validation fails
            ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
            ViewBag.Instructors = _unitOfWork.Instructors.Get().ToList();
            return View(course);
        }

        // GET: Course/Edit/5
        public IActionResult Edit(int id)
        {
            var course = _unitOfWork.Courses.GetOne(
                c => c.Id == id,
                includeProps: query => query.Include(c => c.Category).Include(c => c.Instructor)
            );

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
            ViewBag.Instructors = _unitOfWork.Instructors.Get().ToList();

            return View(course);
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Course course , IFormFile? file)
        {
            if (id != course.Id)
            {
                return BadRequest();
            }
            ModelState.Remove("ImgUrl");
            ModelState.Remove("Category");
            ModelState.Remove("Instructor");

            var oldCourse = _unitOfWork.Courses.GetOne(e => e.Id == course.Id, tracked: false);
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Genereate name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Save in wwwroot
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Course", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    //delete old img
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Course", oldCourse.ImgUrl);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                    // Save in db
                    course.ImgUrl = fileName;
                }
                else
                {
                    course.ImgUrl = oldCourse.ImgUrl;
                }


                _unitOfWork.Courses.Alter(course);
                _unitOfWork.Commit();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _unitOfWork.Categories.Get().ToList();
            ViewBag.Instructors = _unitOfWork.Instructors.Get().ToList();
            return View(course);
        }

        // GET: Course/Delete/5
        public IActionResult Delete(int id)
        {
            var course = _unitOfWork.Courses.GetOne(
                c => c.Id == id,
                includeProps: query => query.Include(c => c.Category).Include(c => c.Instructor)
            );

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var course = _unitOfWork.Courses.GetOne(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            _unitOfWork.Courses.Delete(course);
            _unitOfWork.Commit();
            return RedirectToAction(nameof(Index));
        }
    }
}
