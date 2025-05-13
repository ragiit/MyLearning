using Bulky.Models.Models;

namespace BulkyWeb.Controllers
{
    public class CategoryController(ApplicationDbContext context) : Controller
    {
        public IActionResult Index()
        {
            // Ambil data dari database dan konversi menjadi List<Category>
            List<Category> categories = context.Categories.ToList();
            return View(categories);
        }

        // ... (sisa kode controller Anda) ...

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
                ModelState.AddModelError("Name", "The name cannot be 'test'."); // Gunakan nameof(Category.Name) atau "Name" agar terhubung ke field yang benar
            }

            if (ModelState.IsValid)
            {
                context.Categories.Add(category);
                await context.SaveChangesAsync();

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

            // var category = context.Categories.Find(id); // Find cocok untuk Primary Key
            var category = context.Categories.FirstOrDefault(c => c.Id == id); // Alternatif yang lebih eksplisit
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
                ModelState.AddModelError("Name", "The name cannot be 'test'."); // Gunakan nameof(Category.Name) atau "Name"
            }

            if (ModelState.IsValid)
            {
                context.Categories.Update(category);
                await context.SaveChangesAsync();

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

            var category = await context.Categories.FindAsync(id); // Async find
            if (category == null)
            {
                return NotFound();
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            TempData["success"] = $"Category '{category.Name}' was deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}