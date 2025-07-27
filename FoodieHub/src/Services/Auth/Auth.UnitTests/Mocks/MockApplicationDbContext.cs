namespace Auth.UnitTests.Mocks
{
    public static class MockApplicationDbContext
    {
        public static ApplicationDbContext GetDbContext(Guid userId, string userEmail)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);

            // Seed some data for testing
            if (!dbContext.ApplicationUsers.Any(u => u.Id == userId.ToString()))
            {
                dbContext.ApplicationUsers.Add(new ApplicationUser
                {
                    Id = userId.ToString(),
                    Email = userEmail,
                    NormalizedEmail = userEmail.ToUpperInvariant(),
                    UserName = userEmail,
                    NormalizedUserName = userEmail.ToUpperInvariant(),
                    PasswordHash = "hashedpassword"
                });
                dbContext.SaveChanges();
            }

            return dbContext;
        }
    }
}