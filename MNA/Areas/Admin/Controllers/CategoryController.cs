using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MNA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryController(IRepository<Category> CategoryRepository)
        {
            _categoryRepository = CategoryRepository;
        }

        // GET: Category
        public IActionResult Index()
        {
            var categories = _categoryRepository.Get().ToList();
            return View(categories);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Create(category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: Category/Edit/5
        public IActionResult Edit(int id)
        {
            var category = _categoryRepository.GetOne(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _categoryRepository.Alter(category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: Category/Delete/5
        public IActionResult Delete(int id)
        {
            var category = _categoryRepository.GetOne(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _categoryRepository.GetOne(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            _categoryRepository.Delete(category);
            return RedirectToAction(nameof(Index));
        }
    }
}
