using System.Reflection;

namespace Basket.API.Configuration;

public static class SwaggerConfiguration
{
    public static void AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Basket API - Proyek FoodieHub",
                Version = "v1",
                Description = "Service mikro **Basket API** adalah bagian krusial dalam ekosistem FoodieHub, didedikasikan untuk mengelola keranjang belanja pengguna. " +
                  "API ini menyediakan fungsionalitas untuk menambah, mengubah, dan menghapus item menu, serta mengambil konten keranjang saat ini. " +
                  "Memanfaatkan **Redis** sebagai penyimpanan data sementara berkecepatan tinggi, API ini memastikan pengalaman belanja yang responsif dan efisien.",
                Contact = new OpenApiContact
                {
                    Name = "Tim Pengembang FoodieHub",
                    Email = "developer@foodiehub.com",
                    Url = new Uri("https://github.com/your-github-repo-basket-api") // **Perbarui URL ini ke repositori Basket API Anda**
                },
                License = new OpenApiLicense
                {
                    Name = "Lisensi MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Konfigurasi keamanan JWT untuk Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }
}