namespace Auth.API.Persistence.Seed
{
    public static class SeedData
    {
        public static async Task ApplyMigrationsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (db.Database.GetPendingMigrations().Any())
            {
                await db.Database.MigrateAsync();
            }
        }

        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in IdentityRoles.DefaultRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"✅ Created role: {role}");
                }
            }
        }

        public static async Task SeedUsersAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            const string defaultPassword = "P@ssw0rd";

            foreach (var (role, email, name) in IdentityUsers.DefaultUsers)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user is null)
                {
                    var newUser = new ApplicationUser
                    {
                        Email = email,
                        UserName = email,
                        Name = name,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(newUser, defaultPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, role);
                        Console.WriteLine($"✅ Seeded user {role}: {email}");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"❌ Error seeding {role}: {error.Description}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"ℹ️ User {email} already exists.");
                }
            }
        }
    }
}