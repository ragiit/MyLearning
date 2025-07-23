using System.Reflection;

namespace Menu.API.Configuration;

public static class SwaggerConfiguration
{
    public static void AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Menu API - FoodieHub Project",
                Version = "v1",
                Description = "Mikroservis **Menu API** bertanggung jawab untuk mengelola daftar menu makanan dan kategori di aplikasi FoodieHub. " +
                              "Layanan ini menyediakan fungsionalitas untuk melihat, menambah, mengubah, dan menghapus item menu, " +
                              "serta memfilter berdasarkan berbagai kriteria. Dirancang untuk skalabilitas dan performa.",
                Contact = new OpenApiContact
                {
                    Name = "Tim Pengembang FoodieHub",
                    Email = "developer@foodiehub.com",
                    Url = new Uri("https://github.com/your-github-repo-menu-api") // Ganti URL repo Menu API Anda
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