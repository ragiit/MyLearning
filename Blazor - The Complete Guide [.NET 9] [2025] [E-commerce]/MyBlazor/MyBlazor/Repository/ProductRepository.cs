namespace MyBlazor.Repository
{
    public class ProductRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment) : IProductRepository
    {
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await context.Products.Include(x => x.Category).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await context.Products.Include(p => p.Category).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Product> CreateAsync(Product Product)
        {
            if (Product == null)
            {
                throw new ArgumentNullException(nameof(Product), "Product cannot be null.");
            }
            await context.Products.AddAsync(Product);
            await context.SaveChangesAsync();

            return Product;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var Product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (Product == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(Product.ImageUrl))
            {
                string oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, Product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            context.Products.Remove(Product);
            return (await context.SaveChangesAsync()) > 0;
        }

        public async Task<Product> UpdateAsync(Product Product)
        {
            if (Product == null)
            {
                throw new ArgumentNullException(nameof(Product), "Product cannot be null.");
            }
            context.Products.Update(Product);
            await context.SaveChangesAsync();

            return Product;
        }
    }
}