namespace BulkyWeb.Controllers
{
    public class CategoryController(ApplicationDbContext context) : Controller
    {
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = context.Categories;
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}