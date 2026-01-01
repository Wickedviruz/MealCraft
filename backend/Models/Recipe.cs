namespace MealCraft.Models;

public class Recipe
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int CookTime { get; set; }
    public int Servings { get; set; }
    public string? Difficulty { get; set; }
    public int? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Carbs { get; set; }
    public decimal? Fat { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Loaded separately
    public List<string> Tags { get; set; } = new();
    public List<RecipeIngredientDetail> Ingredients { get; set; } = new();
    public List<Instruction> Instructions { get; set; } = new();
}

public class Ingredient
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class RecipeIngredient
{
    public Guid RecipeId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Amount { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class RecipeIngredientDetail
{
    public Guid IngredientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class Instruction
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public int StepNumber { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Timer { get; set; }
}

public class RecipeTag
{
    public Guid RecipeId { get; set; }
    public string Tag { get; set; } = string.Empty;
}

public class Review
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}