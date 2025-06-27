using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyBlazor.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Books" },
                new Category { Id = 3, Name = "Clothing" },
                new Category { Id = 4, Name = "Toys" },
                new Category { Id = 5, Name = "Home & Kitchen" },
                new Category { Id = 6, Name = "Beauty" },
                new Category { Id = 7, Name = "Sports" },
                new Category { Id = 8, Name = "Automotive" },
                new Category { Id = 9, Name = "Office Supplies" },
                new Category { Id = 10, Name = "Health" }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "Gaming Laptop", Price = 1500, CategoryId = 1 },
                new Product { Id = 2, Name = "Smartphone", Description = "Latest Android phone", Price = 800, CategoryId = 1 },
                new Product { Id = 3, Name = "Novel A", Description = "Fiction Book", Price = 20, CategoryId = 2 },
                new Product { Id = 4, Name = "T-Shirt", Description = "Cotton T-Shirt", Price = 15, CategoryId = 3 },
                new Product { Id = 5, Name = "Toy Car", Description = "Remote controlled car", Price = 30, CategoryId = 4 },
                new Product { Id = 6, Name = "Blender", Description = "Kitchen appliance", Price = 60, CategoryId = 5 },
                new Product { Id = 7, Name = "Lipstick", Description = "Red color lipstick", Price = 10, CategoryId = 6 },
                new Product { Id = 8, Name = "Soccer Ball", Description = "Size 5 official ball", Price = 25, CategoryId = 7 },
                new Product { Id = 9, Name = "Car Vacuum", Description = "Portable vacuum cleaner", Price = 40, CategoryId = 8 },
                new Product { Id = 10, Name = "Notebook", Description = "A5 size notebook", Price = 5, CategoryId = 9 },
                new Product { Id = 11, Name = "Vitamin C", Description = "1000mg tablets", Price = 12, CategoryId = 10 },
                new Product { Id = 12, Name = "Monitor", Description = "27 inch LED monitor", Price = 200, CategoryId = 1 },
                new Product { Id = 13, Name = "Cookbook", Description = "Healthy recipes", Price = 18, CategoryId = 2 },
                new Product { Id = 14, Name = "Jeans", Description = "Slim fit jeans", Price = 35, CategoryId = 3 },
                new Product { Id = 15, Name = "Action Figure", Description = "Superhero collectible", Price = 45, CategoryId = 4 }
            );
        }
    }
}