using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;

namespace MNA.Areas.Student.Controllers
{
    [Area("Student")]
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
        public IActionResult Index(int pageNumber = 1, int numOfItems = 9)
        {
            
            var courses = _unitOfWork.Courses.Get(
                includeProps: query => query.Include(c => c.Category).Include(c => c.Instructor)
            );

            int pages = (int)Math.Ceiling((double)courses.Count() / numOfItems);
            courses = courses.Skip((pageNumber - 1) * numOfItems).Take(numOfItems);
            CoursePaginationVM coursePaginationVM = new CoursePaginationVM()
            {
                Courses = courses.ToList(),
                Pages = pages,
                PageNumber = pageNumber
            };

            return View(model: coursePaginationVM);
        }
    }
}
