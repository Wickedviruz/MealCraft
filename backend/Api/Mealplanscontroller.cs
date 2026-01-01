namespace MealCraft.Api;

using Microsoft.AspNetCore.Mvc;
using MealCraft.Services;
using MealCraft.DTOs;

[ApiController]
[Route("api/[controller]")]
public class MealPlansController : ControllerBase
{
    private readonly MealPlanService _mealPlanService;

    public MealPlansController(MealPlanService mealPlanService)
    {
        _mealPlanService = mealPlanService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MealPlanDto>>> GetMealPlans(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // TODO: Get userId from JWT
        var userId = Guid.NewGuid(); // Fake for now
        
        var mealPlans = await _mealPlanService.GetMealPlans(userId, startDate, endDate);
        return Ok(mealPlans);
    }

    [HttpPost]
    public async Task<ActionResult<MealPlanDto>> AddMealPlan([FromBody] CreateMealPlanDto dto)
    {
        // TODO: Get userId from JWT
        var userId = Guid.NewGuid(); // Fake for now
        
        var mealPlan = await _mealPlanService.AddMealPlan(userId, dto);
        return Ok(mealPlan);
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveMealPlan(
        [FromQuery] DateTime date,
        [FromQuery] string mealType)
    {
        // TODO: Get userId from JWT
        var userId = Guid.NewGuid(); // Fake for now
        
        var deleted = await _mealPlanService.RemoveMealPlan(userId, date, mealType);
        if (!deleted)
            return NotFound(new { message = "Meal plan not found" });

        return NoContent();
    }
}