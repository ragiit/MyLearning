using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;

namespace BulkyWeb.Controllers
{
    public class CategoryController(ICategoryRepository categoryRepository) : Controller
    {
        public IActionResult Index()
        {
            List<Category> categories = [.. categoryRepository.GetAll()];
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
                categoryRepository.Add(category);
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

            var category = categoryRepository.Get(c => c.Id == id);
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
                categoryRepository.Update(category);
                categoryRepository.Save();

                TempData["success"] = $"Category '{category.Name}' was updated successfully.";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // PERHATIAN: Metode Delete sebaiknya menggunakan [HttpPost] untuk keamanan
        // Saya akan biarkan [HttpGet] sesuai kode Anda untuk saat ini, tapi ini tidak direkomendasikan
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = categoryRepository.Get(x => x.Id == id); // Async find
            if (category == null)
            {
                return NotFound();
            }

            categoryRepository.Delete(category);
            categoryRepository.Save();

            TempData["success"] = $"Category '{category.Name}' was deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}