namespace MealCraft.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString();

        // Log incoming request
        _logger.LogInformation(
            "[{RequestId}] {Method} {Path} from {IP}",
            requestId,
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress
        );

        // Continue processing
        await _next(context);

        // Log response
        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
        _logger.LogInformation(
            "[{RequestId}] Completed in {Duration}ms with status {StatusCode}",
            requestId,
            duration,
            context.Response.StatusCode
        );

        // Alert på långsamma requests
        if (duration > 1000)
        {
            _logger.LogWarning(
                "[{RequestId}] SLOW REQUEST: {Method} {Path} took {Duration}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                duration
            );
        }
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}