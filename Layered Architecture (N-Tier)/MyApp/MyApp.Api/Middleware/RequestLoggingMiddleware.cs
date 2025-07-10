using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;
using SerilogLogger = Serilog.ILogger;

namespace MyApp.Api.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
            await next(context);
            stopwatch.Stop();

            logger.LogInformation(
                "Handled {Method} {Path} responded {StatusCode} in {Elapsed:0.0000} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.Elapsed.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex,
                "Error handling request: {Method} {Path} in {Elapsed:0.0000} ms",
                context.Request.Method,
                context.Request.Path,
                stopwatch.Elapsed.TotalMilliseconds);
            throw;
        }
    }
}