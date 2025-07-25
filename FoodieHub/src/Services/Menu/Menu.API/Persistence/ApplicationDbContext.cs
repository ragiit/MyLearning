namespace Menu.API.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Entities.Menu> Menus { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuImage> MenuImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Contoh konfigurasi relasi atau constraint lainnya
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Menus)
                .WithOne(m => m.Category)
                .HasForeignKey(m => m.CategoryId);

            // Pastikan nama kategori unik
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();
        }
    }
}