namespace MealCraft.Database;

using Npgsql;
using Dapper;
using MealCraft.Models;
using MealCraft.DTOs;

public class DatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection") 
            ?? "Host=localhost;Database=mealcraft;Username=mealcraft;Password=dev123";
    }

    private NpgsqlConnection GetConnection() => new(_connectionString);

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
            "SELECT * FROM Users WHERE Id = @Id",
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