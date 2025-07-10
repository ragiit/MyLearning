using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.ResponseCompression;
using MyApp.Business.Validators;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<MyAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register DI
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddMapster();

// FluentValidation
builder.Services
    .AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<ProductValidator>();
    });

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseResponseCaching();
app.UseRateLimiter();
app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapControllers();

ApplyMigration();

app.Run();

void ApplyMigration()
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();

    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }
}