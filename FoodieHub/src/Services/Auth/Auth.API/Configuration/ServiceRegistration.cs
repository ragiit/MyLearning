namespace Auth.API.Configuration;

public static class ServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(Program).Assembly;

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(configuration.GetSection("ApiSettings:JwtOptions"));

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("DefaultConnection")!);

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.SmallestSize;
        });

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

    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secret = configuration["ApiSettings:JwtOptions:Secret"];
        var issuer = configuration["ApiSettings:JwtOptions:Issuer"];
        var audience = configuration["ApiSettings:JwtOptions:Audience"];

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
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
        });
    }
}