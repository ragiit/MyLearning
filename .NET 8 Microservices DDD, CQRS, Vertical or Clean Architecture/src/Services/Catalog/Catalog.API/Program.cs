var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddMarten(x =>
{
    x.Connection(builder.Configuration.GetConnectionString("CatalogDb"));
}).UseLightweightSessions();

var app = builder.Build();

app.MapCarter();

app.Run();