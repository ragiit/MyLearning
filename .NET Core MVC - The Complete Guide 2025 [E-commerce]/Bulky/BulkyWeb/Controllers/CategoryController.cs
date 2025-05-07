namespace BulkyWeb.Controllers
{
    public class CategoryController(ApplicationDbContext context) : Controller
    {
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = context.Categories;
            return View(objCategoryList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (category is not null && category.Name is not null &&   category.Name.Equals("test", StringComparison.CurrentCultureIgnoreCase))
            {
                ModelState.AddModelError("name", "The name cannot be 'test'.");
            }

            if (ModelState.IsValid)
            {
                context.Categories.Add(category);
                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View();
        }
    }
}