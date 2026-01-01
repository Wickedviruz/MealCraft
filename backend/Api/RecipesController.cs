namespace MealCraft.Api;

using Microsoft.AspNetCore.Mvc;
using MealCraft.Services;
using MealCraft.DTOs;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly RecipeService _recipeService;

    public RecipesController(RecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    /// <summary>
    /// Get all recipes with pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<RecipeListDto>>> GetRecipes(
        [FromQuery] int limit = 50, 
        [FromQuery] int offset = 0)
    {
        var recipes = await _recipeService.GetAllRecipes(limit, offset);
        return Ok(recipes);
    }

    /// <summary>
    /// Get recipe by ID with full details
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDto>> GetRecipe(Guid id)
    {
        var recipe = await _recipeService.GetRecipeById(id);
        if (recipe == null) 
            return NotFound(new { message = "Recipe not found" });
        
        return Ok(recipe);
    }

    /// <summary>
    /// Create a new recipe
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RecipeDto>> CreateRecipe([FromBody] CreateRecipeDto dto)
    {
        // TODO: Get userId from JWT token
        var recipe = await _recipeService.CreateRecipe(dto, userId: null);
        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }

    /// <summary>
    /// Update an existing recipe
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<RecipeDto>> UpdateRecipe(Guid id, [FromBody] UpdateRecipeDto dto)
    {
        var recipe = await _recipeService.UpdateRecipe(id, dto);
        if (recipe == null) 
            return NotFound(new { message = "Recipe not found" });
        
        return Ok(recipe);
    }

    /// <summary>
    /// Delete a recipe
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(Guid id)
    {
        var deleted = await _recipeService.DeleteRecipe(id);
        if (!deleted) 
            return NotFound(new { message = "Recipe not found" });
        
        return NoContent();
    }

    /// <summary>
    /// Search recipes
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<List<RecipeListDto>>> SearchRecipes(
        [FromQuery] string query,
        [FromQuery] int limit = 50)
    {
        var recipes = await _recipeService.SearchRecipes(query, limit);
        return Ok(recipes);
    }
}