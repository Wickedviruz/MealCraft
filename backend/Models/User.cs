using System.ComponentModel.DataAnnotations.Schema;

namespace MealCraft.Models;

public class User
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("email")]
    public required string Email { get; set; }

    [Column("passwordhash")]
    public required string PasswordHash { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("avatarurl")]
    public string AvatarUrl { get; set; } = string.Empty;

    [Column("createdat")]
    public DateTime CreatedAt { get; set; }
}
