using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Helper.Role_Admin)]
    public class CategoryController(IUnitOfWork categoryRepository) : Controller
    {
        public IActionResult Index()
        {
            List<Category> categories = [.. categoryRepository.Category.GetAll()];
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (category != null && category.Name != null &&
                category.Name.Equals("test", StringComparison.CurrentCultureIgnoreCase))
            {
                ModelState.AddModelError("Name", "The name cannot be 'test'.");
            }

            if (ModelState.IsValid)
            {
                categoryRepository.Category.Add(category);
                categoryRepository.Save();

                TempData["success"] = $"Category '{category.Name}' was created successfully.";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = categoryRepository.Category.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (category != null && category.Name != null &&
                category.Name.Equals("test", StringComparison.CurrentCultureIgnoreCase))
            {
                ModelState.AddModelError("Name", "The name cannot be 'test'.");
            }

            if (ModelState.IsValid)
            {
                categoryRepository.Category.Update(category);
                categoryRepository.Save();

                TempData["success"] = $"Category '{category.Name}' was updated successfully.";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = categoryRepository.Category.Get(x => x.Id == id); // Async find
            if (category == null)
            {
                return NotFound();
            }

            categoryRepository.Category.Delete(category);
            categoryRepository.Save();

            TempData["success"] = $"Category '{category.Name}' was deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}