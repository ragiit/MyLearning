namespace Apple.Services.ProductAPI.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Apple Watch", Price = 299, Description = "Smartwatch dari Apple", CategoryName = "Wearable", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 2, Name = "iPhone 15", Price = 999, Description = "Smartphone terbaru dari Apple", CategoryName = "Smartphone", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 3, Name = "MacBook Pro", Price = 1999, Description = "Laptop performa tinggi", CategoryName = "Laptop", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 4, Name = "iPad Air", Price = 599, Description = "Tablet ringan dan cepat", CategoryName = "Tablet", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 5, Name = "AirPods Pro", Price = 249, Description = "Earbuds nirkabel dengan noise cancelling", CategoryName = "Aksesoris", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 6, Name = "iMac 24\"", Price = 1299, Description = "All-in-one desktop komputer", CategoryName = "Desktop", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 7, Name = "Apple Pencil", Price = 129, Description = "Stylus untuk iPad", CategoryName = "Aksesoris", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 8, Name = "Magic Mouse", Price = 99, Description = "Mouse wireless dari Apple", CategoryName = "Aksesoris", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 9, Name = "HomePod Mini", Price = 99, Description = "Speaker pintar dari Apple", CategoryName = "Smart Home", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 10, Name = "Apple TV 4K", Price = 179, Description = "Streaming device 4K", CategoryName = "Entertainment", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 11, Name = "Mac Mini", Price = 699, Description = "Komputer kecil namun bertenaga", CategoryName = "Desktop", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 12, Name = "AirTag", Price = 29, Description = "Pelacak barang pintar", CategoryName = "Aksesoris", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 13, Name = "Apple Keyboard", Price = 99, Description = "Keyboard wireless slim", CategoryName = "Aksesoris", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 14, Name = "iPhone SE", Price = 429, Description = "iPhone murah dengan performa tinggi", CategoryName = "Smartphone", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 15, Name = "iPad Mini", Price = 499, Description = "Tablet kecil dengan kekuatan besar", CategoryName = "Tablet", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 16, Name = "Apple Studio Display", Price = 1599, Description = "Layar monitor 5K", CategoryName = "Display", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 17, Name = "Mac Studio", Price = 1999, Description = "Desktop powerful untuk kreator", CategoryName = "Desktop", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 18, Name = "iPhone 14 Pro Max", Price = 1199, Description = "iPhone flagship", CategoryName = "Smartphone", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 19, Name = "Magic Trackpad", Price = 129, Description = "Trackpad besar dan responsif", CategoryName = "Aksesoris", ImageUrl = "https://placehold.co/603x403" },
                new Product { Id = 20, Name = "Apple Watch Ultra", Price = 799, Description = "Smartwatch ekstrem untuk olahraga outdoor", CategoryName = "Wearable", ImageUrl = "https://placehold.co/603x403" }
            );
        }
    }
}