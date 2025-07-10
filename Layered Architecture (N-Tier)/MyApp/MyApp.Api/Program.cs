using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.ResponseCompression;
using MyApp.Api.Middleware;
using MyApp.Business.Validators;
using Serilog;
using System.Threading.RateLimiting;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
// Register DbContext dulu
builder.Services.AddDbContext<MyAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Daftar HealthCheck dan kaitkan ke DbContext
builder.Services.AddHealthChecks()
    .AddDbContextCheck<MyAppDbContext>()
        .AddCheck<DiskSpaceHealthCheck>("Disk Space");

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

builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();

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
app.UseErrorHandling();

app.MapHealthChecksUI();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.ToString()
            })
        };

        await context.Response.WriteAsJsonAsync(result);
    }
});

app.UseRequestLogging();
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