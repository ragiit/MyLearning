using Auth.API.Helper;
using FluentValidation;
using Menu.API.Data;
using Menu.API.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "YourAPI", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header pakai format: 'Bearer {token}'. Contoh: 'Bearer abcdef12345'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
var secret = builder.Configuration["ApiSettings:JwtOptions:Secret"];
var issuer = builder.Configuration["ApiSettings:JwtOptions:Issuer"];
var audience = builder.Configuration["ApiSettings:JwtOptions:Audience"];

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
        };
    });
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.SmallestSize;
});

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
    });
});
var connStringRedis = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddStackExchangeRedisCache(x =>
{
    x.Configuration = connStringRedis;
});
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(connStringRedis);

builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.Decorate<IMenuItemRepository, CachedMenuItemRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseExceptionHandler(options => { });
app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

await ApplyMigrationAsync();
await SeedData.SeedMenuItemsAsync(app.Services);

app.Run();

async Task ApplyMigrationAsync()
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (db.Database.GetPendingMigrations().Any())
    {
        await db.Database.MigrateAsync();
    }
}

public static class SeedData
{
    public static async Task SeedMenuItemsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await db.MenuItems.AnyAsync())
            return;

        var sampleMenus = new List<MenuItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Chicken Teriyaki",
                Categories = new() { "Main Course", "Asian" },
                Price = 45000,
                Carbo = 32,
                Protein = 28,
                Description = "Grilled chicken with teriyaki sauce served with rice.",
                ImageUrl = "https://example.com/images/chicken-teriyaki.jpg"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Avocado Salad",
                Categories = new() { "Salad", "Healthy" },
                Price = 30000,
                Carbo = 10,
                Protein = 3,
                Description = "Fresh avocado salad with olive oil and lemon dressing.",
                ImageUrl = "https://example.com/images/avocado-salad.jpg"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Beef Burger",
                Categories = new() { "Western", "Fast Food" },
                Price = 55000,
                Carbo = 40,
                Protein = 22,
                Description = "Juicy grilled beef burger with cheese and fries.",
                ImageUrl = "https://example.com/images/beef-burger.jpg"
            }
        };

        db.MenuItems.AddRange(sampleMenus);
        await db.SaveChangesAsync();
    }
}