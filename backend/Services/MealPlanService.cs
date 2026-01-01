namespace MealCraft.Services;

using MealCraft.Database;
using MealCraft.Models;
using MealCraft.DTOs;

public class MealPlanService
{
    private readonly DatabaseContext _db;

    public MealPlanService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<List<MealPlanDto>> GetMealPlans(Guid userId, DateTime startDate, DateTime endDate)
    {
        var mealPlans = await _db.GetMealPlans(userId, startDate, endDate);
        var result = new List<MealPlanDto>();

        foreach (var plan in mealPlans)
        {
            var recipe = await _db.GetRecipe(plan.RecipeId);
            if (recipe != null)
            {
                result.Add(new MealPlanDto
                {
                    Id = plan.Id,
                    Date = plan.Date,
                    MealType = plan.MealType,
                    Recipe = new RecipeListDto
                    {
                        Id = recipe.Id,
                        Title = recipe.Title,
                        ImageUrl = recipe.ImageUrl,
                        CookTime = recipe.CookTime,
                        Servings = recipe.Servings,
                        Rating = recipe.Rating,
                        Tags = recipe.Tags
                    }
                });
            }
        }

        return result;
    }

    public async Task<MealPlanDto> AddMealPlan(Guid userId, CreateMealPlanDto dto)
    {
        var mealPlan = new MealPlan
        {
            UserId = userId,
            Date = dto.Date.Date, // Remove time component
            MealType = dto.MealType,
            RecipeId = dto.RecipeId,
            CreatedAt = DateTime.UtcNow
        };

        var id = await _db.CreateMealPlan(mealPlan);
        mealPlan.Id = id;

        var recipe = await _db.GetRecipe(dto.RecipeId);

        return new MealPlanDto
        {
            Id = id,
            Date = mealPlan.Date,
            MealType = mealPlan.MealType,
            Recipe = new RecipeListDto
            {
                Id = recipe!.Id,
                Title = recipe.Title,
                ImageUrl = recipe.ImageUrl,
                CookTime = recipe.CookTime,
                Servings = recipe.Servings,
                Rating = recipe.Rating,
                Tags = recipe.Tags
            }
        };
    }

    public async Task<bool> RemoveMealPlan(Guid userId, DateTime date, string mealType)
    {
        return await _db.DeleteMealPlan(userId, date.Date, mealType);
    }
}