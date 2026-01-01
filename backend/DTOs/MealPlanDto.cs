namespace MealCraft.DTOs;

public class MealPlanDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string MealType { get; set; } = string.Empty;
    public RecipeListDto Recipe { get; set; } = null!;
}

public class CreateMealPlanDto
{
    public DateTime Date { get; set; }
    public string MealType { get; set; } = string.Empty;
    public Guid RecipeId { get; set; }
}

public class ShoppingListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool Checked { get; set; }
}

public class UpdateShoppingItemDto
{
    public bool Checked { get; set; }
}