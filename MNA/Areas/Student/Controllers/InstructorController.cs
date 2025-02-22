using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MNA.Areas.Student.Controllers
{
    [Area("Student")]
    public class InstructorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public InstructorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Profile(int id)
        {
            var instructor = _unitOfWork.Instructors.Get(
                i => i.Id == id,
                includeProps: q => q.Include(sc => sc.Courses)
            ).FirstOrDefault();

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }
    }

}
