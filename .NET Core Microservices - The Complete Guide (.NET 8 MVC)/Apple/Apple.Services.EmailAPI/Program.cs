global using Apple.Services.EmailAPI.Data;
global using Apple.Services.EmailAPI.Models;
global using Apple.Services.EmailAPI.Models.Dto;
global using Apple.Services.EmailAPI.Services;
global using Microsoft.EntityFrameworkCore;
using Apple.Services.EmailAPI.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 🟢 Register DbContext dulu
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🟢 Register EmailService (opsional: bisa juga jadi scoped atau transient)
builder.Services.AddScoped<IEmailService, EmailService>();

// 🟢 Hosted service (background RabbitMQ consumer)
builder.Services.AddHostedService<RabbitMqAuthConsumer>();
builder.Services.AddHostedService<RabbitMqCartConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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