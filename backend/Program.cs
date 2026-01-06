using System.Threading.RateLimiting;
using MealCraft.Database;
using MealCraft.Services;
using MealCraft.Middleware;
using MealCraft.Utils;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Bind till alla nätverksgränssnitt
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Validera att kritiska settings finns
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string not configured. " +
        "Copy appsettings.Development.json.example to appsettings.Development.json and configure it."
    );
}

var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 32)
{
    throw new InvalidOperationException(
        "JWT Secret must be configured and at least 32 characters long. " +
        "Set it in appsettings.Development.json"
    );
}

// ratelimiting (din kod oförändrad)
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var userIdentifier = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userIdentifier,
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 50,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            });
    });

    rateLimiterOptions.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests",
            message = "Rate limit exceeded. Please try again later."
        }, cancellationToken: cancellationToken);
    };
});

// CORS - Uppdaterad version
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.SetIsOriginAllowed(origin => 
                {
                    var uri = new Uri(origin);
                    return uri.Host == "localhost" 
                        || uri.Host == "127.0.0.1"
                        || uri.Host.StartsWith("192.168.")
                        || uri.Host.StartsWith("10.0.")
                        || uri.Host.StartsWith("172.");
                })
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
        else
        {
            policy.WithOrigins("https://www.johanivarsson.se")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

// Services (oförändrat)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "MealCraft API", 
        Version = "v1",
        Description = "Meal planning and recipe management API"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtHelper>();
//builder.Services.AddScoped<RecipeService>();
//builder.Services.AddScoped<MealPlanService>();
//builder.Services.AddScoped<ShoppingListService>();


builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection") 
            ?? "Host=localhost;Database=mealcraft;Username=mealcraft;Password=dev123",
        name: "database",
        timeout: TimeSpan.FromSeconds(3));

var app = builder.Build();

app.UseSecurityHeaders();
app.UseRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowVue");
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

Console.WriteLine("MealCraft API starting...");
Console.WriteLine($"API:        http://0.0.0.0:5000/api");
Console.WriteLine($"Swagger:    http://0.0.0.0:5000/swagger");
Console.WriteLine($"Health:     http://0.0.0.0:5000/health");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

// Skriv ut alla nätverksadresser
var addresses = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
    .Where(ni => ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
    .SelectMany(ni => ni.GetIPProperties().UnicastAddresses)
    .Where(addr => addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
    .Select(addr => addr.Address.ToString());

Console.WriteLine("\nListening on:");
foreach (var addr in addresses)
{
    Console.WriteLine($"  http://{addr}:5000");
}

app.Run();