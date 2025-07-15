namespace Ordering.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            return services;
        }

        public static WebApplication UseApplicationServices(this WebApplication application)
        {
            return application;
        }
    }
}