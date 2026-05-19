using AuthSystem.Identity.DTOs;
using AuthSystem.Identity.Models;
using AuthSystem.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthSystem.Identity.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService) : ControllerBase
{
    /// <summary>Register a new user. Assigned "User" role by default.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var user = new ApplicationUser
        {
            FullName       = req.FullName,
            Email          = req.Email,
            UserName       = req.UserName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(ApiResponse.Fail(
                string.Join(", ", result.Errors.Select(e => e.Description))));

        await userManager.AddToRoleAsync(user, "User");

        return Ok(ApiResponse.Ok(new { user.Id, user.Email }, "Registration successful"));
    }

    /// <summary>Login and receive a JWT token.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await userManager.FindByEmailAsync(req.Email);

        if (user is null || !user.IsActive)
            return Unauthorized(ApiResponse.Fail("Invalid credentials"));

        if (!await userManager.CheckPasswordAsync(user, req.Password))
            return Unauthorized(ApiResponse.Fail("Invalid credentials"));

        var (token, expiresAt) = await tokenService.GenerateTokenAsync(user);
        var roles              = await userManager.GetRolesAsync(user);
        var userClaims         = await userManager.GetClaimsAsync(user);
        var permissions        = userClaims.Where(c => c.Type == "Permission").Select(c => c.Value);

        return Ok(ApiResponse.Ok(new AuthResponse(
            token,
            user.Email!,
            user.FullName,
            roles,
            permissions,
            expiresAt
        )));
    }
}
