namespace MealCraft.Services;

using MealCraft.Database;
using MealCraft.Models;
using MealCraft.DTOs;
using BCrypt.Net;
using MealCraft.Utils;

public class UserService
{
    private readonly DatabaseContext _db;

    public UserService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<Result<AuthResponseDto>> Register(RegisterDto dto)
    {
        // Check if user exists
        var existing = await _db.GetUserByEmail(dto.Email);
        if (existing != null)
            {
                return new Result<AuthResponseDto>
                {
                    Success = false,
                    ErrorMessage = "User already exists",
                    ErrorType = ErrorType.Conflict
                };
            }

        // Hash password
        var passwordHash = BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = passwordHash,
            Name = dto.Name,
            CreatedAt = DateTime.UtcNow,
        };

        var userId = await _db.CreateUser(user);
        user.Id = userId;

        // TODO: Generate JWT token
        var token = "fake-jwt-token";

        return new Result<AuthResponseDto>
        {
            Success = true,
            Data = new AuthResponseDto { user = MapToDto(user), token = token, refreshToken = token }
        };
    }

    public async Task<AuthResponseDto?> Login(LoginDto dto)
    {
        var user = await _db.GetUserByEmail(dto.Email);
        if (user == null) return null;

        // Verify password
        if (!BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

        // TODO: Generate JWT token
        var token = "fake-jwt-token";

        return new AuthResponseDto
        {
            user = MapToDto(user),
            token = token,
            refreshToken = token
        };
    }

    public async Task<UserDto?> GetUserById(Guid id)
    {
        var user = await _db.GetUserById(id);
        return user != null ? MapToDto(user) : null;
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            CreatedAt = user.CreatedAt
        };
    }
}