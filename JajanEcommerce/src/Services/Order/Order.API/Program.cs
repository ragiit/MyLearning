using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.Messaging.MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Order.Application;
using Order.Application.Consumer;
using Order.Domain.Repositories;
using Order.Infrastructure;
using Order.Infrastructure.Repositories;
using System.Security.Claims;

//using Order.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;
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
    }); builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddControllers();
builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration);
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddMessageBroker(
builder.Configuration,
typeof(BasketCheckoutEventConsumer).Assembly
);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseResponseCompression();
//app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseExceptionHandler(options => { });
//app.UseHealthChecks("/health",
//    new HealthCheckOptions
//    {
//        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//    });

await ApplyMigrationAsync();

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

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => httpContextAccessor.HttpContext?.User?.Identity?.Name;
}

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
}