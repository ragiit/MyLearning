namespace Menu.API.Configuration
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(Program).Assembly;

            // MediatR Configuration
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            // FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            // Database Context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Exception Handler
            services.AddExceptionHandler<CustomExceptionHandler>();

            // Health Checks
            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection")!, name: "MenuDB-Check");

            // Response Compression
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.SmallestSize;
            });

            // Rate Limiting
            services.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
                {
                    options.Window = TimeSpan.FromSeconds(10);
                    options.PermitLimit = 5;
                });
            });

            services.AddControllers();
        }

        // Menambahkan konfigurasi JWT Authentication untuk memvalidasi token dari Auth Microservice
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var secret = configuration["ApiSettings:JwtOptions:Secret"];
            var issuer = configuration["ApiSettings:JwtOptions:Issuer"];
            var audience = configuration["ApiSettings:JwtOptions:Audience"];

            if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT configuration (Secret, Issuer, Audience) is missing in appsettings.");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

            // Opsi untuk mengambil JwtOptions dari konfigurasi (jika dibutuhkan di bagian lain)
            services.Configure<JwtOptions>(configuration.GetSection("ApiSettings:JwtOptions"));
        }
    }
}