using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MealCraft.DTOs;

#region request bodys
// ============================================
// Request bodys
// ============================================

/// <summary>
/// User registration request
/// </summary>
public class RegisterDto
{
    // User's full name
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public required string Name { get; set; }

    // User's email address (used for login)
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }

    // User's password (minimum 6 characters)
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public required string Password { get; set; } 
}

/// <summary>
/// User login request
/// </summary>
public class LoginDto
{

    // User's email address (used for login)
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }

    // User's password (minimum 6 characters)
    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; set; } 
}

/// <summary>
/// refresh access token
/// </summary>
public class RefreshTokenDto
{
    [Required]
    public required string RefreshToken { get; set; }
}

public class ResetPwDto
{
    [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public required string NewPassword { get; set; }
}
#endregion

#region datagroups for responses
// ============================================
// Datagroups for reponses
// ============================================

/// <summary>
/// User data in signup response
/// </summary>
public class SignupUserDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// User data in login response
/// </summary>
public class LoginUserDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}

/// <summary>
/// Forgot password request
/// </summary>
public class ForgotPasswordDto
{
    [Required, EmailAddress]
    public required string Email { get; set; }
}

/// <summary>
/// Reset password request
/// </summary>
public class ResetPasswordDto
{
    // password reset token
    [Required]
    public required string Token { get; set; }

    // new password (min 6 characters)
    [Required,MinLength(6)]
    public required string NewPassword { get; set; }
}
#endregion

#region Responses
// ============================================
// Responses
// ============================================

/// <summary>
/// User signup response
/// </summary>
public class SignupResponseDto
{
    [JsonPropertyName("user")]
    public SignupUserDto User { get; set; } = null!;
    
    [JsonPropertyName("token")]
    public required string Token { get; set; }
    
    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; set; }
}

/// <summary>
/// User login response
/// </summary>
public class LoginResponseDto
{
    [JsonPropertyName("user")]
    public LoginUserDto User { get; set; } = null!;
    
    [JsonPropertyName("token")]
    public required string Token { get; set; }
    
    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; set; }
}

public class RefreshTokenResponseDto
{
    [JsonPropertyName("token")]
    public required string Token { get; set; }
    
    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; set; }
}

#endregion