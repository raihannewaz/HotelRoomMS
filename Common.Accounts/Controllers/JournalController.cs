using System.IdentityModel.Tokens.Jwt;
using AuthSystem.Identity.DTOs;
using AuthSystem.Identity.Models;
using AuthSystem.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Common.Accounts.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.UsersView)]
    public async Task<IActionResult> GetAll()
    {
        var users  = await userManager.Users.ToListAsync();
        var result = new List<UserDto>();

        foreach (var u in users)
        {
            var roles = await userManager.GetRolesAsync(u);
            result.Add(new UserDto(u.Id, u.FullName, u.Email!, u.IsActive, u.CreatedAt, roles));
        }

        return Ok(ApiResponse.Ok(result));
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.UsersView)]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound(ApiResponse.Fail("User not found"));

        var roles = await userManager.GetRolesAsync(user);
        return Ok(ApiResponse.Ok(new UserDto(
            user.Id, user.FullName, user.Email!, user.IsActive, user.CreatedAt, roles)));
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UsersEdit)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest req)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound(ApiResponse.Fail("User not found"));

        if (req.FullName is not null) user.FullName = req.FullName;
        if (req.IsActive is not null) user.IsActive  = req.IsActive.Value;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(ApiResponse.Fail(
                string.Join(", ", result.Errors.Select(e => e.Description))));

        return Ok(ApiResponse.Ok<object?>(null, "User updated"));
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.UsersDelete)]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound(ApiResponse.Fail("User not found"));

        await userManager.DeleteAsync(user);
        return Ok(ApiResponse.Ok<object?>(null, "User deleted"));
    }

    [HttpPost("{id}/roles")]
    [HasPermission(Permissions.UsersEdit)]
    public async Task<IActionResult> AssignRoles(string id, [FromBody] AssignRolesRequest req)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound(ApiResponse.Fail("User not found"));

        var current = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, current);

        var result = await userManager.AddToRolesAsync(user, req.Roles);
        if (!result.Succeeded)
            return BadRequest(ApiResponse.Fail(
                string.Join(", ", result.Errors.Select(e => e.Description))));

        return Ok(ApiResponse.Ok<object?>(null, "Roles assigned"));
    }

    /// <summary>Returns the currently authenticated user's profile + permissions.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                  ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (userId is null) return Unauthorized();

        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var roles       = await userManager.GetRolesAsync(user);
        var permissions = User.Claims
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value);

        return Ok(ApiResponse.Ok(new
        {
            user.Id,
            user.FullName,
            user.Email,
            user.IsActive,
            user.CreatedAt,
            Roles       = roles,
            Permissions = permissions
        }));
    }
}
