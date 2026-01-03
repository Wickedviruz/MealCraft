using System.Threading.RateLimiting;
using MealCraft.Database;
using MealCraft.Services;
using MealCraft.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ratelimiting
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    // Global rate limit per IP address
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

    // Response när rate limit nås
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

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "MealCraft API", 
        Version = "v1",
        Description = "Meal planning and recipe management API"
    });
});

// CORS for Vue frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Database
builder.Services.AddSingleton<DatabaseContext>();

// Services
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<MealPlanService>();
builder.Services.AddScoped<ShoppingListService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection") 
            ?? "Host=localhost;Database=mealcraft;Username=mealcraft;Password=dev123",
        name: "database",
        timeout: TimeSpan.FromSeconds(3));

var app = builder.Build();

app.UseSecurityHeaders();

app.UseRequestLogging();

// Configure pipeline
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
Console.WriteLine($"API:        http://localhost:5000/api");
Console.WriteLine($"Swagger:    http://localhost:5000/swagger");
Console.WriteLine($"Health:     http://localhost:5000/health");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

app.Run();