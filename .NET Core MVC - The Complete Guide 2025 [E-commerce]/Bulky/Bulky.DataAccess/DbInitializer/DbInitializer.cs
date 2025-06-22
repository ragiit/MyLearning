using Bulky.DataAccess.Data;
using Bulky.Models.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.DbInitializer
{
    public class DbInitializer(UserManager<IdentityUser> _userManager, RoleManager<IdentityRole> _roleManager, ApplicationDbContext _db) : IDbInitializer
    {
        public async Task Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch { }

            if (!_roleManager.RoleExistsAsync(Helper.Role_Customer).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(Helper.Role_Customer));
                await _roleManager.CreateAsync(new IdentityRole(Helper.Role_Company));
                await _roleManager.CreateAsync(new IdentityRole(Helper.Role_Admin));
                await _roleManager.CreateAsync(new IdentityRole(Helper.Role_Employee));
                await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "Admin",
                    PhoneNumber = "1234567890",
                    StreetAddress = "123 Admin St",
                    City = "Admin City",
                    State = "Admin State",
                    PostalCode = "12345",
                }, "P@ssw0rd123");

                var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");
                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, Helper.Role_Admin);
                }
            }

            return;
        }

        public void SeedData()
        {
            // Implementation for seeding initial data into the database
        }

        public void MigrateDatabase()
        {
            // Implementation for migrating the database schema
        }

        public void EnsureCreated()
        {
            // Implementation to ensure the database is created
        }

        public void EnsureDeleted()
        {
            // Implementation to ensure the database is deleted
        }

        public void ApplyMigrations()
        {
            // Implementation to apply migrations to the database
        }

        public void CreateDatabaseIfNotExists()
        {
            // Implementation to create the database if it does not exist
        }

        public void ConfigureDatabaseSettings()
        {
            // Implementation to configure database settings
        }

        public void LogDatabaseInitializationStatus()
        {
            // Implementation to log the status of database initialization
        }
    }
}