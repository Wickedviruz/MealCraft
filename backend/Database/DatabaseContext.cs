namespace MealCraft.Database;

using Npgsql;
using Dapper;
using MealCraft.Models;
using MealCraft.DTOs;

public class DatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection") 
            ?? "Host=localhost;Database=mealcraft;Username=mealcraft;Password=dev123";
    }

    private NpgsqlConnection GetConnection() => new(_connectionString);

    // ============================================
    // RECIPES
    // ============================================
    
    public async Task<List<Recipe>> GetRecipes(int limit = 50, int offset = 0)
    {
        using var conn = GetConnection();
        var recipes = (await conn.QueryAsync<Recipe>(
            "SELECT * FROM Recipes ORDER BY CreatedAt DESC LIMIT @Limit OFFSET @Offset",
            new { Limit = limit, Offset = offset }
        )).ToList();
        
        foreach (var recipe in recipes)
        {
            await LoadRecipeRelations(conn, recipe);
        }
        
        return recipes;
    }

    public async Task<Recipe?> GetRecipe(Guid id)
    {
        using var conn = GetConnection();
        var recipe = await conn.QueryFirstOrDefaultAsync<Recipe>(
            "SELECT * FROM Recipes WHERE Id = @Id",
            new { Id = id }
        );
        
        if (recipe != null)
        {
            await LoadRecipeRelations(conn, recipe);
        }
        
        return recipe;
    }

    private async Task LoadRecipeRelations(NpgsqlConnection conn, Recipe recipe)
    {
        // Load tags
        recipe.Tags = (await conn.QueryAsync<string>(
            "SELECT Tag FROM RecipeTags WHERE RecipeId = @RecipeId",
            new { RecipeId = recipe.Id }
        )).ToList();
        
        // Load ingredients with details
        recipe.Ingredients = (await conn.QueryAsync<RecipeIngredientDetail>(@"
            SELECT 
                ri.IngredientId,
                i.Name,
                ri.Amount,
                ri.Unit,
                i.Category
            FROM RecipeIngredients ri
            JOIN Ingredients i ON ri.IngredientId = i.Id
            WHERE ri.RecipeId = @RecipeId",
            new { RecipeId = recipe.Id }
        )).ToList();
        
        // Load instructions
        recipe.Instructions = (await conn.QueryAsync<Instruction>(
            "SELECT * FROM Instructions WHERE RecipeId = @RecipeId ORDER BY StepNumber",
            new { RecipeId = recipe.Id }
        )).ToList();
    }

    public async Task<Guid> CreateRecipe(Recipe recipe, List<string> tags, List<CreateIngredientDto> ingredients, List<CreateInstructionDto> instructions)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            // Insert recipe
            var recipeId = await conn.QuerySingleAsync<Guid>(@"
                INSERT INTO Recipes (UserId, Title, Description, ImageUrl, CookTime, Servings, Difficulty, Calories, Protein, Carbs, Fat, CreatedAt, UpdatedAt)
                VALUES (@UserId, @Title, @Description, @ImageUrl, @CookTime, @Servings, @Difficulty, @Calories, @Protein, @Carbs, @Fat, @CreatedAt, @UpdatedAt)
                RETURNING Id",
                recipe,
                transaction
            );

            // Insert tags
            foreach (var tag in tags)
            {
                await conn.ExecuteAsync(
                    "INSERT INTO RecipeTags (RecipeId, Tag) VALUES (@RecipeId, @Tag)",
                    new { RecipeId = recipeId, Tag = tag },
                    transaction
                );
            }

            // Insert ingredients
            foreach (var ing in ingredients)
            {
                // Get or create ingredient
                var ingredientId = await conn.QueryFirstOrDefaultAsync<Guid?>(
                    "SELECT Id FROM Ingredients WHERE LOWER(Name) = LOWER(@Name)",
                    new { ing.Name },
                    transaction
                );

                if (ingredientId == null)
                {
                    ingredientId = await conn.QuerySingleAsync<Guid>(
                        "INSERT INTO Ingredients (Name, Category) VALUES (@Name, @Category) RETURNING Id",
                        new { ing.Name, ing.Category },
                        transaction
                    );
                }

                await conn.ExecuteAsync(
                    "INSERT INTO RecipeIngredients (RecipeId, IngredientId, Amount, Unit) VALUES (@RecipeId, @IngredientId, @Amount, @Unit)",
                    new { RecipeId = recipeId, IngredientId = ingredientId, ing.Amount, ing.Unit },
                    transaction
                );
            }

            // Insert instructions
            for (int i = 0; i < instructions.Count; i++)
            {
                await conn.ExecuteAsync(
                    "INSERT INTO Instructions (RecipeId, StepNumber, Text, Timer) VALUES (@RecipeId, @StepNumber, @Text, @Timer)",
                    new { RecipeId = recipeId, StepNumber = i + 1, instructions[i].Text, instructions[i].Timer },
                    transaction
                );
            }

            await transaction.CommitAsync();
            return recipeId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateRecipe(Guid id, UpdateRecipeDto dto)
    {
        using var conn = GetConnection();
        
        var updates = new List<string>();
        var parameters = new DynamicParameters();
        parameters.Add("Id", id);
        parameters.Add("UpdatedAt", DateTime.UtcNow);

        if (dto.Title != null) { updates.Add("Title = @Title"); parameters.Add("Title", dto.Title); }
        if (dto.Description != null) { updates.Add("Description = @Description"); parameters.Add("Description", dto.Description); }
        if (dto.ImageUrl != null) { updates.Add("ImageUrl = @ImageUrl"); parameters.Add("ImageUrl", dto.ImageUrl); }
        if (dto.CookTime.HasValue) { updates.Add("CookTime = @CookTime"); parameters.Add("CookTime", dto.CookTime); }
        if (dto.Servings.HasValue) { updates.Add("Servings = @Servings"); parameters.Add("Servings", dto.Servings); }
        if (dto.Difficulty != null) { updates.Add("Difficulty = @Difficulty"); parameters.Add("Difficulty", dto.Difficulty); }
        if (dto.Calories.HasValue) { updates.Add("Calories = @Calories"); parameters.Add("Calories", dto.Calories); }
        if (dto.Protein.HasValue) { updates.Add("Protein = @Protein"); parameters.Add("Protein", dto.Protein); }
        if (dto.Carbs.HasValue) { updates.Add("Carbs = @Carbs"); parameters.Add("Carbs", dto.Carbs); }
        if (dto.Fat.HasValue) { updates.Add("Fat = @Fat"); parameters.Add("Fat", dto.Fat); }

        if (updates.Count == 0) return false;

        updates.Add("UpdatedAt = @UpdatedAt");

        var sql = $"UPDATE Recipes SET {string.Join(", ", updates)} WHERE Id = @Id";
        var affected = await conn.ExecuteAsync(sql, parameters);
        
        return affected > 0;
    }

    public async Task<bool> DeleteRecipe(Guid id)
    {
        using var conn = GetConnection();
        var affected = await conn.ExecuteAsync("DELETE FROM Recipes WHERE Id = @Id", new { Id = id });
        return affected > 0;
    }

    // ============================================
    // USERS
    // ============================================

    public async Task<User?> GetUserByEmail(string email)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email",
            new { Email = email }
        );
    }

    public async Task<User?> GetUserById(Guid id)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Id = @Id",
            new { Id = id }
        );
    }

    public async Task<Guid> CreateUser(User user)
    {
        using var conn = GetConnection();
        return await conn.QuerySingleAsync<Guid>(
            "INSERT INTO Users (Username, Email, PasswordHash, CreatedAt, UpdatedAt) VALUES (@Username, @Email, @PasswordHash, @CreatedAt, @UpdatedAt) RETURNING Id",
            user
        );
    }

    // ============================================
    // MEAL PLANS
    // ============================================

    public async Task<List<MealPlan>> GetMealPlans(Guid userId, DateTime startDate, DateTime endDate)
    {
        using var conn = GetConnection();
        return (await conn.QueryAsync<MealPlan>(
            "SELECT * FROM MealPlans WHERE UserId = @UserId AND Date >= @StartDate AND Date <= @EndDate ORDER BY Date, MealType",
            new { UserId = userId, StartDate = startDate, EndDate = endDate }
        )).ToList();
    }

    public async Task<Guid> CreateMealPlan(MealPlan mealPlan)
    {
        using var conn = GetConnection();
        return await conn.QuerySingleAsync<Guid>(
            @"INSERT INTO MealPlans (UserId, Date, MealType, RecipeId, CreatedAt) 
              VALUES (@UserId, @Date, @MealType, @RecipeId, @CreatedAt)
              ON CONFLICT (UserId, Date, MealType) DO UPDATE SET RecipeId = @RecipeId
              RETURNING Id",
            mealPlan
        );
    }

    public async Task<bool> DeleteMealPlan(Guid userId, DateTime date, string mealType)
    {
        using var conn = GetConnection();
        var affected = await conn.ExecuteAsync(
            "DELETE FROM MealPlans WHERE UserId = @UserId AND Date = @Date AND MealType = @MealType",
            new { UserId = userId, Date = date, MealType = mealType }
        );
        return affected > 0;
    }

    // ============================================
    // SHOPPING LIST
    // ============================================

    public async Task<List<ShoppingListItem>> GetShoppingList(Guid userId)
    {
        using var conn = GetConnection();
        return (await conn.QueryAsync<ShoppingListItem>(
            "SELECT * FROM ShoppingLists WHERE UserId = @UserId ORDER BY Checked, CreatedAt",
            new { UserId = userId }
        )).ToList();
    }

    public async Task<Guid> AddToShoppingList(ShoppingListItem item)
    {
        using var conn = GetConnection();
        return await conn.QuerySingleAsync<Guid>(
            "INSERT INTO ShoppingLists (UserId, IngredientId, Amount, Unit, Checked, CreatedAt) VALUES (@UserId, @IngredientId, @Amount, @Unit, @Checked, @CreatedAt) RETURNING Id",
            item
        );
    }

    public async Task<bool> UpdateShoppingItem(Guid id, bool isChecked)
    {
        using var conn = GetConnection();
        var affected = await conn.ExecuteAsync(
            "UPDATE ShoppingLists SET Checked = @Checked WHERE Id = @Id",
            new { Id = id, Checked = isChecked }
        );
        return affected > 0;
    }

    public async Task<bool> DeleteCheckedItems(Guid userId)
    {
        using var conn = GetConnection();
        var affected = await conn.ExecuteAsync(
            "DELETE FROM ShoppingLists WHERE UserId = @UserId AND Checked = true",
            new { UserId = userId }
        );
        return affected > 0;
    }
}