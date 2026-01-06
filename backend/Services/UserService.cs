namespace MealCraft.Services;

using MealCraft.Database;
using MealCraft.Models;
using MealCraft.DTOs;
using MealCraft.Utils;

public class UserService
{
    private readonly DatabaseContext _db;
    private readonly JwtHelper _jwtHelper;

    public UserService(DatabaseContext db, JwtHelper jwtHelper)
    {
        _db = db;
        _jwtHelper = jwtHelper;
    }
}