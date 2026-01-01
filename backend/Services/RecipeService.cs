namespace MealCraft.Services;

using MealCraft.Database;
using MealCraft.Models;
using MealCraft.DTOs;

public class RecipeService
{
    private readonly DatabaseContext _db;

    public RecipeService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<List<RecipeListDto>> GetAllRecipes(int limit = 50, int offset = 0)
    {
        var recipes = await _db.GetRecipes(limit, offset);
        return recipes.Select(MapToListDto).ToList();
    }

    public async Task<RecipeDto?> GetRecipeById(Guid id)
    {
        var recipe = await _db.GetRecipe(id);
        return recipe != null ? MapToDto(recipe) : null;
    }

    public async Task<RecipeDto> CreateRecipe(CreateRecipeDto dto, Guid? userId = null)
    {
        var recipe = new Recipe
        {
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            CookTime = dto.CookTime,
            Servings = dto.Servings,
            Difficulty = dto.Difficulty,
            Calories = dto.Calories,
            Protein = dto.Protein,
            Carbs = dto.Carbs,
            Fat = dto.Fat,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var id = await _db.CreateRecipe(recipe, dto.Tags, dto.Ingredients, dto.Instructions);
        
        var createdRecipe = await _db.GetRecipe(id);
        return MapToDto(createdRecipe!);
    }

    public async Task<RecipeDto?> UpdateRecipe(Guid id, UpdateRecipeDto dto)
    {
        var success = await _db.UpdateRecipe(id, dto);
        if (!success) return null;
        
        var recipe = await _db.GetRecipe(id);
        return recipe != null ? MapToDto(recipe) : null;
    }

    public async Task<bool> DeleteRecipe(Guid id)
    {
        return await _db.DeleteRecipe(id);
    }

    public async Task<List<RecipeListDto>> SearchRecipes(string query, int limit = 50)
    {
        // TODO: Implement search
        return await GetAllRecipes(limit, 0);
    }

    // ============================================
    // MAPPING: Model → DTO
    // ============================================

    private RecipeDto MapToDto(Recipe recipe)
    {
        return new RecipeDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            ImageUrl = recipe.ImageUrl,
            CookTime = recipe.CookTime,
            Servings = recipe.Servings,
            Difficulty = recipe.Difficulty,
            Calories = recipe.Calories,
            Protein = recipe.Protein,
            Carbs = recipe.Carbs,
            Fat = recipe.Fat,
            Rating = recipe.Rating,
            ReviewCount = recipe.ReviewCount,
            Tags = recipe.Tags,
            Ingredients = recipe.Ingredients.Select(i => new IngredientDto
            {
                Id = i.IngredientId,
                Name = i.Name,
                Amount = i.Amount,
                Unit = i.Unit,
                Category = i.Category
            }).ToList(),
            Instructions = recipe.Instructions.Select(inst => new InstructionDto
            {
                StepNumber = inst.StepNumber,
                Text = inst.Text,
                Timer = inst.Timer
            }).ToList(),
            CreatedAt = recipe.CreatedAt
        };
    }

    private RecipeListDto MapToListDto(Recipe recipe)
    {
        return new RecipeListDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            ImageUrl = recipe.ImageUrl,
            CookTime = recipe.CookTime,
            Servings = recipe.Servings,
            Rating = recipe.Rating,
            Tags = recipe.Tags
        };
    }
}