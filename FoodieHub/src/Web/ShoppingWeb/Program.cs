using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.FeatureManagement.FeatureFilters;
using Microsoft.IdentityModel.Tokens;
using Refit;
using ShoppingWeb.Components;
using ShoppingWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRefitClient<IAuthApi>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiUrls:GatewayUrl"] ?? "http://146.190.102.184:7777");
    });
//var secret = builder.Configuration["ApiSettings:JwtOptions:Secret"];
//var issuer = builder.Configuration["ApiSettings:JwtOptions:Issuer"];
//var audience = builder.Configuration["ApiSettings:JwtOptions:Audience"];
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//        .AddJwtBearer("Bearer", options =>
//        {
//            options.TokenValidationParameters = new TokenValidationParameters
//            {
//                ValidateIssuerSigningKey = true,
//                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret)),
//                ValidateIssuer = true,
//                ValidIssuer = issuer,
//                ValidateAudience = true,
//                ValidAudience = audience,
//                ValidateLifetime = true,
//                ClockSkew = TimeSpan.Zero,
//            };
//        });
builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddScheme<CustomOption, JWTAuthenticationHandler>("JWT", x =>
{
});
builder.Services.AddScoped<CookieService>();
builder.Services.AddScoped<AccessTokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JWTAuthenticationStateProvider>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AuthenticationStateProvider, JWTAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();