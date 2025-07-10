using Mapster;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Interfaces;
using MyApp.Data;
using MyApp.Data.Repositories;
using MyApp.Service.Interfaces;
using MyApp.Service.Services;
using System.Reflection;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 1. Konfigurasi DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Registrasi Repositories dan Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

// 3. Registrasi Services
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();

// 4. Registrasi Mapster
builder.Services.AddMapster();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCaching();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "fixed", opt =>
    {
        opt.PermitLimit = 20; // Jumlah request maksimum
        opt.Window = TimeSpan.FromMinutes(1); // Dalam rentang waktu
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0; // Jumlah request yang antri jika limit tercapai
    });

    // Memberikan response 429 Too Many Requests saat limit terlampaui
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
builder.Services.AddResponseCompression(options =>
{
    // Aktifkan kompresi Brotli, yang lebih baik dari Gzip
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// Konfigurasi level kompresi untuk Brotli
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseResponseCaching();
app.UseRateLimiter();
app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

ApplyMigration();

app.Run();

void ApplyMigration()
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }
}