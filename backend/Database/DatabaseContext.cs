namespace MealCraft.Database;

using Npgsql;
using Dapper;
using MealCraft.Models;
using MealCraft.DTOs;
using System.Security.Cryptography;

public class DatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured."); 
    }

    private NpgsqlConnection GetConnection() => new(_connectionString);

    // ============================================
    // Auth
    // ============================================

    public async Task<string> CreateRefreshToken(Guid userId, int daysValid = 7)
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(tokenBytes);

        var expiresAt = DateTime.UtcNow.AddDays(daysValid);
        var createdAt = DateTime.UtcNow;

        using var conn = GetConnection();
        await conn.ExecuteAsync(
            "INSERT INTO refresh_tokens (user_id, token, expires_at, created_at) VALUES (@UserId, @Token, @ExpiresAt, @CreatedAt)",
            new { 
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt,
                CreatedAt = createdAt
            }
        );
        return token;
    }

    // Validera refresh token
    public async Task<Guid?> ValidateRefreshToken(string token)
    {
        using var conn = GetConnection();

        var refreshToken = await conn.QueryFirstOrDefaultAsync<RefreshToken>(
            @"SELECT * FROM refresh_tokens 
            WHERE token = @Token
            AND expires_at > @Now 
            AND revoked_at IS NULL",
            new { Token = token, Now = DateTime.UtcNow }
        );
        
        return refreshToken?.UserId;
    }

     // Revoke refresh token
    public async Task RevokeRefreshTokensForUser(Guid userId)
    {
        using var conn = GetConnection();

        var revokeToken = await conn.ExecuteAsync(
            @"UPDATE refresh_tokens
            SET revoked_at = @Now  
            WHERE user_id = @UserId
            AND revoked_at IS NULL",
            new { UserId = userId, Now = DateTime.UtcNow }
        );
    }

    public async Task<Guid?> ResetPassword(User user)
    {
        using var conn = GetConnection();

        // change SQL to reset the password
        return await conn.QuerySingleAsync<Guid>(
            "INSERT INTO Users WHERE (Email, PasswordHash, Name, AvatarUrl, CreatedAt) VALUES (@Email, @PasswordHash, @Name, @AvatarUrl, @CreatedAt) RETURNING Id",
            user
        );
    }

    // ============================================
    // USERS
    // ============================================

    public async Task<User?> GetUserByEmail(string email)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email",
            new { Email = email }
        );
    }

    public async Task<User?> GetUserById(Guid id)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE id = @Id",
            new { Id = id }
        );
    }

    public async Task<Guid> CreateUser(User user)
    {
        using var conn = GetConnection();
        return await conn.QuerySingleAsync<Guid>(
            "INSERT INTO Users (Email, PasswordHash, Name, AvatarUrl, CreatedAt) VALUES (@Email, @PasswordHash, @Name, @AvatarUrl, @CreatedAt) RETURNING Id",
            user
        );
    }
}