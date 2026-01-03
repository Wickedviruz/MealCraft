# MealCraft Backend API

ASP.NET Core Web API med PostgreSQL och Dapper.

## Struktur

```bash
backend/
    Models/           # Database entities
    DTOs/            # API contracts (Data Transfer Objects)
    Api/             # Controllers
    Services/        # Business logic
    Middleware/      # Middlewares
    Database/        # Database context & migrations
    Program.cs       # Entry point
    appsettings.json # Configuration
```

## Setup

### 1. PostgreSQL Database

```bash

```

### 2. Run Migrations

```bash
bashpsql -h localhost -U mealcraft -d mealcraft -f Database/schema.sql
# Password: dev123
```

### 3. Restore & Build

```bash
dotnet restore
dotnet build
```

### 4. Run API

```bash
dotnet run
API runs on: http://localhost:5000
Swagger UI: http://localhost:5000/swagger
```

## API Endpoints

### Recipes

GET /api/recipes - List all recipes
GET /api/recipes/{id} - Get recipe details
POST /api/recipes - Create recipe
PUT /api/recipes/{id} - Update recipe
DELETE /api/recipes/{id} - Delete recipe
GET /api/recipes/search?query=pasta - Search recipes

### Users

POST /api/users/register - Register new user
POST /api/users/login - Login
GET /api/users/{id} - Get user profile

### Meal Plans

GET /api/mealplans?startDate=2024-01-01&endDate=2024-01-07 - Get meal plans
POST /api/mealplans - Add meal to calendar
DELETE /api/mealplans?date=2024-01-01&mealType=lunch - Remove meal

### Shopping List

GET /api/shoppinglists - Get shopping list
PATCH /api/shoppinglists/{id} - Toggle item checked
DELETE /api/shoppinglists/checked - Clear checked items
POST /api/shoppinglists/generate - Generate from meal plan

## DTO Pattern

Models = Database representation (internal)
DTOs = API contracts (public)

### Example

```bash
csharp// Model (has sensitive data)
public class User {
    public string PasswordHash { get; set; } // Never exposed!
}
```

// DTO (safe to return)

```bash
public class UserDto {
    public string Username { get; set; } // Only public data
}
```

## Development

- Hot reload: dotnet watch run
- Format code: dotnet format
- Test API: Use Swagger UI at /swagger

## Environment Variables

```bash
ConnectionStrings__DefaultConnection="Host=localhost;Database=mealcraft;Username=mealcraft;Password=dev123"
```

## TODO

- JWT authentication
- Image upload to cloud storage
- Recipe search implementation
- Nutrition calculation
- Email verification
