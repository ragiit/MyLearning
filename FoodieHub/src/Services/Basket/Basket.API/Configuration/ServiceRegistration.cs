using Refit;
using StackExchange.Redis;
using System.Net.Http.Headers;

namespace Basket.API.Configuration
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
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IConnectionMultiplexer>(
              ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection") ??
                                            throw new InvalidOperationException("RedisConnection string is not configured.")));

            // Exception Handler
            services.AddExceptionHandler<CustomExceptionHandler>();

            // Health Checks
            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("RedisConnection")!);

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

            services.AddHttpContextAccessor();

            services.AddRefitClient<IMenuApiClient>()
                  .ConfigureHttpClient((serviceProvider, client) =>
                  {
                      var menuApiUrl = configuration["ApiSettings:MenuApiUrl"] ??
                                       throw new InvalidOperationException("MenuApiUrl is not configured.");
                      client.BaseAddress = new Uri(menuApiUrl);
                  })
                  .AddHttpMessageHandler(serviceProvider =>
                  {
                      var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                      return new AuthorizationHeaderHandler(httpContextAccessor);
                  }).ConfigurePrimaryHttpMessageHandler(() =>
                  {
                      return new HttpClientHandler
                      {
                          ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                      };
                  });

            services.AddScoped<IBasketRepository, BasketRepository>();

            services.AddControllers();
        }

        public class AuthorizationHeaderHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var accessToken = httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();

                if (!string.IsNullOrEmpty(accessToken))
                {
                    if (accessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Substring("Bearer ".Length));
                    }
                    else
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }
                }
                return base.SendAsync(request, cancellationToken);
            }
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