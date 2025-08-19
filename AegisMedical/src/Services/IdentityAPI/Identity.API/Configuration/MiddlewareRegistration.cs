namespace Identity.API.Configuration;

public static class MiddlewareRegistration
{
    public static void UseCustomMiddlewares(this IApplicationBuilder app)
    {
        //app.UseHttpsRedirection();

        app.UseResponseCompression();

        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseExceptionHandler(_ => { });

        app.UseHealthChecks("/health",
            new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
            });
    }
}