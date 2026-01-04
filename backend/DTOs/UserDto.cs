using System.ComponentModel.DataAnnotations;

namespace MealCraft.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RegisterDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public required string Name { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public required string Password { get; set; } 
}

public class LoginDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; set; } 
}

public class AuthResponseDto
{
    public required string token { get; set; }
    public required string refreshToken { get; set; }
    public UserDto user { get; set; } = null!;
}