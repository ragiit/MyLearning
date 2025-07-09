using Apple.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Apple.Services.CouponAPI.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Reward> Rewards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}