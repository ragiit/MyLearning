namespace Apple.Services.CouponAPI.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                Id = 1,
                Code = "10OFF",
                Discount = 10,
                MinAmount = 20
            });

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                Id = 2,
                Code = "20OFF",
                Discount = 20,
                MinAmount = 40
            });
        }
    }
}