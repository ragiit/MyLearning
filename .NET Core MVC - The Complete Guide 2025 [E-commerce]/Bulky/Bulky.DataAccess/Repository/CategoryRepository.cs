using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository
{
    public class CategoryRepository(ApplicationDbContext context) : Repository<Category>(context), ICategoryRepository
    {
        public void Update(Category category)
        {
            context.Categories.Update(category);
        }
    }
}