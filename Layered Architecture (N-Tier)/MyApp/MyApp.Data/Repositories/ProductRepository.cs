using Microsoft.EntityFrameworkCore;

namespace MyApp.Data.Repositories
{
    public class ProductRepository(MyAppDbContext context) : IProductRepository
    {
        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = context.Products.AsNoTracking();
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Id)
                .Paged(pageNumber, pageSize)
                .ToListAsync();
            return (items, totalCount);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await context.Products.FindAsync(id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            context.Products.Update(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
            }
        }
    }
}