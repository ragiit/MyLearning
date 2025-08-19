using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Identity.API.Configuration;

/// <summary>
/// Konfigurasi untuk Swagger (OpenAPI) documentation.
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Menambahkan dan mengonfigurasi SwaggerGen dengan dukungan otentikasi JWT.
    /// </summary>
    public static void AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Identity API - AegisMedical",
                Version = "v1",
                Description = "Layanan **Identity API** adalah gerbang keamanan utama untuk platform **AegisMedical**. " +
                              "API ini bertanggung jawab penuh atas manajemen identitas, otentikasi (login), dan otorisasi (hak akses) pengguna, " +
                              "seperti dokter, staf administrasi, dan manajemen. Dengan menerbitkan **JSON Web Token (JWT)** yang aman, " +
                              "layanan ini memastikan bahwa hanya pengguna yang sah yang dapat mengakses data medis sensitif di seluruh ekosistem microservice AegisMedical.",
                Contact = new OpenApiContact
                {
                    Name = "Tim Pengembang AegisMedical",
                    Email = "developer@aegismedical.com",
                    Url = new Uri("https://github.com/your-repo/aegismedical-identity-api") // Ganti dengan URL repo Anda
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Konfigurasi keamanan JWT untuk Swagger UI
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Otorisasi header JWT menggunakan skema Bearer. Masukkan 'Bearer' [spasi] lalu token Anda. Contoh: 'Bearer 12345abcdef'",
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
                        }
                    },
                    new List<string>()
                }
            });

            // Mengaktifkan komentar XML untuk dokumentasi yang lebih kaya di Swagger
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }
}