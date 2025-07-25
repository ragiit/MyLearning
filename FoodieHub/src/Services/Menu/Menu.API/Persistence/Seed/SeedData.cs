﻿namespace Menu.API.Persistence.Seed
{
    public static class SeedData
    {
        public static async Task ApplyMigrationsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        public static async Task SeedCategoriesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!await dbContext.Categories.AnyAsync())
            {
                dbContext.Categories.AddRange(
                    new Category { Name = "Main Course", Description = "Hidangan utama" },
                    new Category { Name = "Appetizers", Description = "Hidangan pembuka" },
                    new Category { Name = "Drinks", Description = "Berbagai minuman" }
                );
                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedMenusAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!await dbContext.Menus.AnyAsync())
            {
                var mainCourseCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Main Course");
                var drinksCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Drinks");

                if (mainCourseCategory != null && drinksCategory != null)
                {
                    dbContext.Menus.AddRange(
                        new Entities.Menu
                        {
                            Name = "Nasi Goreng Spesial",
                            Description = "Nasi goreng dengan ayam, udang, dan telur.",
                            Price = 25000m,
                            CategoryId = mainCourseCategory.Id,
                            IsAvailable = true
                        },
                        new Entities.Menu
                        {
                            Name = "Es Teh Manis",
                            Description = "Es teh dengan gula asli.",
                            Price = 8000m,
                            CategoryId = drinksCategory.Id,
                            IsAvailable = true
                        }
                    );
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}