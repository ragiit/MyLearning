var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await SeedData.ApplyMigrationsAsync(app.Services);
await SeedData.SeedRolesAsync(app.Services);
await SeedData.SeedUsersAsync(app.Services);

app.UseCustomMiddlewares();

app.MapControllers();

app.Run();