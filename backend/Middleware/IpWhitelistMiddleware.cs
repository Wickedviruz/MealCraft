namespace MealCraft.Middleware;

public class IpWhitelistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpWhitelistMiddleware> _logger;
    private readonly HashSet<string> _allowedIps;

    public IpWhitelistMiddleware(
        RequestDelegate next, 
        ILogger<IpWhitelistMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        
        // Läs tillåtna IPs från config
        var allowedIps = configuration.GetSection("Security:AllowedIPs").Get<string[]>() 
            ?? Array.Empty<string>();
        
        _allowedIps = new HashSet<string>(allowedIps);
        
        // Alltid tillåt localhost
        _allowedIps.Add("127.0.0.1");
        _allowedIps.Add("::1");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var remoteIp = context.Connection.RemoteIpAddress?.ToString();

        if (remoteIp != null && !_allowedIps.Contains(remoteIp))
        {
            _logger.LogWarning("Blocked request from IP: {IP}", remoteIp);
            context.Response.StatusCode = 403; // Forbidden
            await context.Response.WriteAsync("Access denied.");
            return;
        }

        await _next(context);
    }
}

public static class IpWhitelistMiddlewareExtensions
{
    public static IApplicationBuilder UseIpWhitelist(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<IpWhitelistMiddleware>();
    }
}