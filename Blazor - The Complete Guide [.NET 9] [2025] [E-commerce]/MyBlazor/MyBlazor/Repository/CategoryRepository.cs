using Microsoft.EntityFrameworkCore;

namespace MyBlazor.Repository
{
    public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
    {
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return false;
            }

            context.Categories.Remove(category);
            return (await context.SaveChangesAsync()) > 0;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }
            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return category;
        }
    }
}