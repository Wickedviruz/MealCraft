namespace MealCraft.DTOs;

// Response DTO - What API returns
public class RecipeDto
{
    public Guid Id { get; set; }
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
    public List<string> Tags { get; set; } = new();
    public List<IngredientDto> Ingredients { get; set; } = new();
    public List<InstructionDto> Instructions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class RecipeListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int CookTime { get; set; }
    public int Servings { get; set; }
    public decimal Rating { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class IngredientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class InstructionDto
{
    public int StepNumber { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Timer { get; set; }
}

// Create DTO - What comes IN to create
public class CreateRecipeDto
{
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
    public List<string> Tags { get; set; } = new();
    public List<CreateIngredientDto> Ingredients { get; set; } = new();
    public List<CreateInstructionDto> Instructions { get; set; } = new();
}

public class CreateIngredientDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class CreateInstructionDto
{
    public string Text { get; set; } = string.Empty;
    public string? Timer { get; set; }
}

// Update DTO - Partial updates
public class UpdateRecipeDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? CookTime { get; set; }
    public int? Servings { get; set; }
    public string? Difficulty { get; set; }
    public int? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Carbs { get; set; }
    public decimal? Fat { get; set; }
}

// Review DTOs
public class CreateReviewDto
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}