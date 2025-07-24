var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await SeedData.ApplyMigrationsAsync(app.Services);
await SeedData.SeedCategoriesAsync(app.Services);
await SeedData.SeedMenusAsync(app.Services);

app.UseCustomMiddlewares();

app.MapControllers();

app.Run();