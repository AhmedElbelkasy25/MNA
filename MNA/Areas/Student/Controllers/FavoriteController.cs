using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace MNA.Areas.Student.Controllers
{
    [Area("student")]
    public class FavoriteController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoriteController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var favoriteCourses = _unitOfWork.Favourites
                .Get(f => f.ApplicationUserId == userId, includeProps: q => q.Include(f => f.Course).ThenInclude(c => c.Instructor))
                .Select(f => f.Course)
                .ToList();

            return View(favoriteCourses);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToFavorites(int courseId)
        {
            var userId = _userManager.GetUserId(User);
            var existingFavorite = _unitOfWork.Favourites.GetOne(f => f.ApplicationUserId == userId && f.CourseId == courseId);

            if (existingFavorite == null)
            {
                var favorite = new Favourite
                {
                    ApplicationUserId = userId,
                    CourseId = courseId
                };

                _unitOfWork.Favourites.Create(favorite);
                _unitOfWork.Commit();
            }

            return RedirectToAction("Index", "Favorite", new { area = "Student" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int courseId)
        {
            var userId = _userManager.GetUserId(User);
            var favorite = _unitOfWork.Favourites.GetOne(f => f.ApplicationUserId == userId && f.CourseId == courseId);

            if (favorite != null)
            {
                _unitOfWork.Favourites.Delete(favorite);
                _unitOfWork.Commit();
            }

            return RedirectToAction("Index", "Favorite", new { area = "Student" });
        }
    }
}
