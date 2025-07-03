global using Apple.Web.Models;
global using Apple.Web.Service.IService;
global using Newtonsoft.Json;
global using System.Text;
using Apple.Web.Service;
using Apple.Web.Utility;

var builder = WebApplication.CreateBuilder(args);
SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<ICouponService, CouponService>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICouponService, CouponService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();