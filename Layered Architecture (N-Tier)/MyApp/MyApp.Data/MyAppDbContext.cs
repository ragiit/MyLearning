namespace MyApp.Data
{
    public class MyAppDbContext(DbContextOptions<MyAppDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
    }
}