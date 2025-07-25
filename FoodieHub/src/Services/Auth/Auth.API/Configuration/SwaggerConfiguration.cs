﻿using System.Reflection;

namespace Auth.API.Configuration;

public static class SwaggerConfiguration
{
    public static void AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Auth API - Proyek FoodieHub",
                Version = "v1",
                Description = "Service mikro **Auth API** berfungsi sebagai pusat autentikasi dan otorisasi untuk aplikasi FoodieHub. " +
                    "API ini menangani semua aspek identitas pengguna, termasuk pendaftaran pengguna baru, proses login yang aman, dan penerbitan " +
                    "**JSON Web Token (JWT)** serta refresh token. Dirancang untuk keamanan dan skalabilitas, API ini adalah pintu gerbang " +
                    "bagi akses pengguna ke seluruh layanan FoodieHub.",
                Contact = new OpenApiContact
                {
                    Name = "Tim Pengembang FoodieHub",
                    Email = "developer@foodiehub.com",
                    Url = new Uri("https://github.com/your-github-repo-auth-api") // **Perbarui URL ini ke repositori Auth API Anda**
                },
                License = new OpenApiLicense
                {
                    Name = "Lisensi MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header pakai format: 'Bearer {token}'. Contoh: 'Bearer abc123'.",
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