namespace MealCraft.Services;

using MealCraft.Database;
using MealCraft.Models;
using MealCraft.DTOs;
using BCrypt.Net;
using MealCraft.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

public class AuthService
{
    private readonly DatabaseContext _db;
    private readonly JwtHelper _jwtHelper;

    public AuthService(DatabaseContext db, JwtHelper jwtHelper)
    {
        _db = db;
        _jwtHelper = jwtHelper;
    }

    public async Task<Result<SignupResponseDto>> Register(RegisterDto dto)
    {
        // Check if user exists
        var existing = await _db.GetUserByEmail(dto.Email);
        if (existing != null)
            {
                return new Result<SignupResponseDto>
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

        var token = _jwtHelper.GenerateToken(user.Id, user.Email, user.Name);
        var refreshToken = await _db.CreateRefreshToken(user.Id, daysValid: 7);

        return new Result<SignupResponseDto>
        {
            Success = true,
            Data = new SignupResponseDto 
            { 
                User = new SignupUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt
                },
                Token = token,
                RefreshToken = refreshToken
            }
        };
    }

    /// <summary>
    /// Login function
    /// </summary>
    public async Task<Result<LoginResponseDto>> Login(LoginDto dto)
    {
        var user = await _db.GetUserByEmail(dto.Email);
        if (user == null || !BCrypt.Verify(dto.Password, user.PasswordHash) ) 
            {
                return new Result<LoginResponseDto>
                {
                    Success = false,
                    ErrorMessage = "Invalid credentials",
                    ErrorType = ErrorType.InvalidCredentials
                };
            }

        var token = _jwtHelper.GenerateToken(user.Id, user.Email, user.Name);
        var refreshToken = await _db.CreateRefreshToken(user.Id, daysValid: 7);

        return new Result<LoginResponseDto>
        {
            Success = true,
            Data = new LoginResponseDto
            { 
                User = new LoginUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                },
                Token = token,
                RefreshToken = refreshToken
            }
        };
    }

    /// <summary>
    /// Logout function
    /// </summary>
    public async Task<Result<bool>> Logout(string jwtToken)
    {
        var userId = _jwtHelper.ValidateToken(jwtToken);

        if (userId == null)
        {
            return new Result<bool>
            {
                Success = false,
                ErrorMessage = "Invalid or expired token",
                ErrorType = ErrorType.InvalidCredentials
            };
        }
        await _db.RevokeRefreshTokensForUser(userId.Value);

        return new Result<bool>
        {
            Success = true,
            Data = true
        };
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    public async Task<Result<RefreshTokenResponseDto>> RefreshAccessToken(RefreshTokenDto dto)
    {

        // 1. Validera refresh token i databas (finns? ej utgången? ej revoked?)
        //    Hint: Du har redan ValidateRefreshToken() metod!
        var userId = await _db.ValidateRefreshToken(dto.RefreshToken);
        Console.WriteLine($"ValidateRefreshToken returned: {userId}");

        if (userId == null )
        {
            Console.WriteLine("userId is NULL");
            return new Result<RefreshTokenResponseDto>
            {
                Success = false,
                ErrorMessage = "Invalid or expired token",
                ErrorType = ErrorType.InvalidCredentials
            };
        }
        
        // 2. Om invalid → returnera error
        
        // 3. Hämta User från databas (behövs för att generera JWT)
        //    Hint: Du har GetUserById()
        Console.WriteLine($"Calling GetUserById with: {userId.Value}");
        var user = await _db.GetUserById(userId.Value);
        Console.WriteLine($"GetUserById returned: {(user == null ? "NULL" : user.Email)}");
            if (user == null )
            {
                Console.WriteLine("User is NULL!");
                return new Result<RefreshTokenResponseDto>
                {
                    Success = false,
                    ErrorMessage = "User not found",
                    ErrorType = ErrorType.InvalidCredentials
                };
            }
            
        // 4. Generera ny JWT access token
        var token = _jwtHelper.GenerateToken(user.Id, user.Email, user.Name);

        // 5. Revoke gamla refresh token
        await _db.RevokeRefreshTokensForUser(user.Id);
        
        // 6. Skapa ny refresh token
        var refreshToken = await _db.CreateRefreshToken(user.Id, daysValid: 7);

        // 7. Returnera båda nya tokens
        return new Result<RefreshTokenResponseDto>
        {
            Success = true,
            Data = new RefreshTokenResponseDto
            {
                Token = token,
                RefreshToken = refreshToken
            }
        };
    }
}