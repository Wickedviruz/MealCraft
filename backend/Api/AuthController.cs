namespace MealCraft.Controllers;

using Microsoft.AspNetCore.Mvc;
using MealCraft.Services;
using MealCraft.Utils;
using MealCraft.DTOs;

/// <summary>
/// Authentication endpoints for user signup, login, and token management
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Create a new user account
    /// </summary>
    /// <param name="dto">User registration details</param>
    /// <returns>Authentication response with tokens</returns>
    /// <response code="201">User created successfully</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="409">Email already exists</response>
    [HttpPost("signup")]
    public async Task<ActionResult<SignupResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.Register(dto);
        if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.Conflict => Conflict(new { message = result.ErrorMessage }),
                    ErrorType.InvalidCredentials => Unauthorized(new { message = result.ErrorMessage }),
                    ErrorType.InvalidInput => BadRequest(new { message = result.ErrorMessage }),
                    ErrorType.InternalError => StatusCode(500, new { message = result.ErrorMessage }),
                    _ => StatusCode(500, new { message = "Unknown error" })
                };
            }

        return StatusCode(201, result.Data);
    }


    /// <summary>
    /// Authenticate an existing user
    /// </summary>
    /// <param name="dto">Login credentials (email and password)</param>
    /// <returns>Authentication response with JWT token and refresh token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.Login(dto);
        if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.InvalidCredentials => Unauthorized(new { message = result.ErrorMessage }),
                    ErrorType.InternalError => StatusCode(500, new { message = result.ErrorMessage }),
                    _ => StatusCode(500, new { message = "Unknown error" })
                };
            }

        return Ok(result.Data);
    }

    /// <summary>
    /// Refreshes a users token
    /// </summary>
    /// <param name="dto">refreshtoken</param>
    /// <returns>response with new JWT token and refresh token</returns>
    /// <response code="200">JWT token and refreshtoken</response>
    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponseDto>> Refresh([FromBody] RefreshTokenDto dto)
    {
        // requestbody kommer med refreshtoken
        var result = await _authService.RefreshAccessToken(dto);
        if (!result.Success)
        {
            return Unauthorized(new { message = result.ErrorMessage });
        }
        // return 200 new JWT token & samma refreshtoken
        return Ok(result.Data);
    }


    /// <summary>
    /// Invalidate the current session and revoke refresh token
    /// </summary>
    /// <returns>No content</returns>
    /// <response code="204">Session invalidated successfully</response>
    /// <response code="401">Not authenticated</response>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // invalidate current session token
        var authHeader = Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Unauthorized(new { message = "No token provided" });
        }
        
        var token = authHeader.Substring(7);
        
        var result = await _authService.Logout(token);
        

        if (!result.Success)
        {
            return Unauthorized(new { message = result.ErrorMessage });
        }

        // return 204 no content
        return NoContent();
    }


    /// <summary>
    /// Forgot password
    /// </summary>
    /// <param name="dto">Forgot password</param>
    /// <response code="200">Password reset email sent</response>
    [HttpPost("forgot-password")]
    public async Task<ActionResult<IActionResult>> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        // request body kommer med email
        // return 200 "password reset email sent"
        return Ok("password reset email sent");
    }


    /// <summary>
    /// resets password
    /// </summary>
    /// <param name="dto">Reset userpassword</param>
    /// <response code="200">Password reset successful</response>
    [HttpPost("reset-password")]
    public async Task<ActionResult<IActionResult>> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        // requestbody kommer med token och "newpassword"
        // return 200 "password reset successful"
        return Ok("password reset successful");
    }
}

