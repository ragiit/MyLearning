global using Apple.Services.CouponAPI.Data;
global using Apple.Services.CouponAPI.Models;
global using Apple.Services.CouponAPI.Models.Dto;
global using Mapster;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMapster();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
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