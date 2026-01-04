namespace MealCraft.Api;

using Microsoft.AspNetCore.Mvc;
using MealCraft.Services;
using MealCraft.Utils;
using MealCraft.DTOs;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var result = await _userService.Register(dto);
        if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.Conflict => Conflict(new { message = result.ErrorMessage }),
                    ErrorType.InvalidCredentials => Unauthorized(new { message = result.ErrorMessage }),
                    ErrorType.InvalidInput => BadRequest(new { message = result.ErrorMessage }),
                    ErrorType.InternalError => StatusCode(500, new { message = result.ErrorMessage }),
                    _ => StatusCode(500, new { message = "Unknown error" })
                };
            }

        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _userService.Login(dto);
        if (result == null)
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await _userService.GetUserById(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(user);
    }
}