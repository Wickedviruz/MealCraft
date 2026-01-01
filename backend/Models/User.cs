namespace MealCraft.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class MealPlan
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public string MealType { get; set; } = string.Empty; // breakfast, lunch, dinner, etc
    public Guid RecipeId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ShoppingListItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Amount { get; set; }
    public string Unit { get; set; } = string.Empty;
    public bool Checked { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SavedRecipe
{
    public Guid UserId { get; set; }
    public Guid RecipeId { get; set; }
    public DateTime SavedAt { get; set; }
}