namespace MealCraft.Api;

using Microsoft.AspNetCore.Mvc;
using MealCraft.Services;
using MealCraft.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ShoppingListsController : ControllerBase
{
    private readonly ShoppingListService _shoppingListService;

    public ShoppingListsController(ShoppingListService shoppingListService)
    {
        _shoppingListService = shoppingListService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShoppingListItemDto>>> GetShoppingList()
    {
        // TODO: Get userId from JWT
        var userId = Guid.NewGuid(); // Fake for now
        
        var items = await _shoppingListService.GetShoppingList(userId);
        return Ok(items);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> ToggleItem(Guid id, [FromBody] UpdateShoppingItemDto dto)
    {
        var updated = await _shoppingListService.ToggleItem(id, dto.Checked);
        if (!updated)
            return NotFound(new { message = "Item not found" });

        return NoContent();
    }

    [HttpDelete("checked")]
    public async Task<IActionResult> ClearCheckedItems()
    {
        // TODO: Get userId from JWT
        var userId = Guid.NewGuid(); // Fake for now
        
        await _shoppingListService.ClearCheckedItems(userId);
        return NoContent();
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateFromMealPlan(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // TODO: Get userId from JWT
        var userId = Guid.NewGuid(); // Fake for now
        
        await _shoppingListService.GenerateFromMealPlan(userId, startDate, endDate);
        return Ok(new { message = "Shopping list generated" });
    }
}