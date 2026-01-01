using MealCraft.Database;
using MealCraft.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowVue");
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("MealCraft API starting...");
Console.WriteLine($"Running on: http://localhost:5000");

app.Run();