using Discount.Grpc.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data
{
    public class DiscountContext(DbContextOptions<DiscountContext> options) : DbContext(options)
    {
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon
                {
                    Id = 1,
                    ProductName = "IPhone 14",
                    Descripcion = "Discount for iPhone 14",
                    Amount = 150
                },
                new Coupon
                {
                    Id = 2,
                    ProductName = "Samsung Galaxy S23",
                    Descripcion = "Discount for Samsung Galaxy",
                    Amount = 120
                },
                new Coupon
                {
                    Id = 3,
                    ProductName = "MacBook Pro",
                    Descripcion = "Student Discount for MacBook",
                    Amount = 200
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}