using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Data;

namespace MyApp.Tests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove real DB
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MyAppDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Pakai SQLite InMemory
            services.AddDbContext<MyAppDbContext>(options =>
            {
                options.UseSqlite("Filename=:memory:");
            });

            // Buat schema SQLite in-memory
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
            db.Database.OpenConnection();
            db.Database.EnsureCreated();
        });
    }
}