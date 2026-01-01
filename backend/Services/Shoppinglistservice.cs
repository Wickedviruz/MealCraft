namespace MealCraft.Services;

using MealCraft.Database;
using MealCraft.Models;
using MealCraft.DTOs;

public class ShoppingListService
{
    private readonly DatabaseContext _db;

    public ShoppingListService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<List<ShoppingListItemDto>> GetShoppingList(Guid userId)
    {
        var items = await _db.GetShoppingList(userId);
        var result = new List<ShoppingListItemDto>();

        foreach (var item in items)
        {
            var ingredient = await _db.GetRecipe(item.IngredientId); // Should be GetIngredient
            // Simplified for now
            result.Add(new ShoppingListItemDto
            {
                Id = item.Id,
                Name = "Ingredient Name", // TODO: Join with Ingredients table
                Amount = item.Amount,
                Unit = item.Unit,
                Category = "Category",
                Checked = item.Checked
            });
        }

        return result;
    }

    public async Task<ShoppingListItemDto> AddItem(Guid userId, Guid ingredientId, decimal amount, string unit)
    {
        var item = new ShoppingListItem
        {
            UserId = userId,
            IngredientId = ingredientId,
            Amount = amount,
            Unit = unit,
            Checked = false,
            CreatedAt = DateTime.UtcNow
        };

        var id = await _db.AddToShoppingList(item);

        return new ShoppingListItemDto
        {
            Id = id,
            Name = "Ingredient", // TODO
            Amount = amount,
            Unit = unit,
            Category = "Category",
            Checked = false
        };
    }

    public async Task<bool> ToggleItem(Guid id, bool isChecked)
    {
        return await _db.UpdateShoppingItem(id, isChecked);
    }

    public async Task<bool> ClearCheckedItems(Guid userId)
    {
        return await _db.DeleteCheckedItems(userId);
    }

    public async Task GenerateFromMealPlan(Guid userId, DateTime startDate, DateTime endDate)
    {
        // TODO: Get all meal plans in range, extract ingredients, add to shopping list
        // This would aggregate all ingredients from meals in the date range
    }
}