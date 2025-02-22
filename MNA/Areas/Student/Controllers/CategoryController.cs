﻿using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MNA.Areas.Student.Controllers
{
    [Area("Student")]
    public class StudentCategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentCategoriesController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var applicationUserId = _userManager.GetUserId(User);

            if (applicationUserId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Fetch student categories with related category data
            var studentCategories = _unitOfWork.StudentCategories.Get(
                sc => sc.ApplicationUserId == applicationUserId,
                includeProps: q => q.Include(sc => sc.Category)
            ).ToList();

            if (!studentCategories.Any())
            {
                ViewBag.Message = "You have not selected any categories yet.";
            }

            return View(studentCategories);
        }
    }
}