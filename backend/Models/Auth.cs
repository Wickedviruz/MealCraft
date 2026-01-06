using System.ComponentModel.DataAnnotations.Schema;

namespace MealCraft.Models;

public class RefreshToken
{
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }
    public required string Token { get; set; }

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }
}
