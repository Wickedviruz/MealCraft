namespace MealCraft.Services;

using MealCraft.Database;
using MealCraft.Models;
using MealCraft.DTOs;
using BCrypt.Net;

public class UserService
{
    private readonly DatabaseContext _db;

    public UserService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<AuthResponseDto?> Register(RegisterDto dto)
    {
        // Check if user exists
        var existing = await _db.GetUserByEmail(dto.Email);
        if (existing != null) return null;

        // Hash password
        var passwordHash = BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var userId = await _db.CreateUser(user);
        user.Id = userId;

        // TODO: Generate JWT token
        var token = "fake-jwt-token";

        return new AuthResponseDto
        {
            Token = token,
            User = MapToDto(user)
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
            Token = token,
            User = MapToDto(user)
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
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
}