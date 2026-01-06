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

}