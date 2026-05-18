using System.Security.Claims;
using AuthSystem.Identity.DTOs;
using AuthSystem.Identity.Models;
using AuthSystem.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Common.Accounts.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize]
public class GroupController(RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.RolesView)]
    public async Task<IActionResult> GetAll()
    {
        var roles  = await roleManager.Roles.ToListAsync();
        var result = new List<RoleDto>();

        foreach (var role in roles)
        {
            var claims      = await roleManager.GetClaimsAsync(role);
            var permissions = claims.Where(c => c.Type == "Permission").Select(c => c.Value);
            result.Add(new RoleDto(role.Id, role.Name!, permissions));
        }

        return Ok(ApiResponse.Ok(result));
    }

    [HttpPost]
    [HasPermission(Permissions.RolesCreate)]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest req)
    {
        if (await roleManager.RoleExistsAsync(req.Name))
            return Conflict(ApiResponse.Fail("Role already exists"));

        var result = await roleManager.CreateAsync(new IdentityRole(req.Name));
        if (!result.Succeeded)
            return BadRequest(ApiResponse.Fail(
                string.Join(", ", result.Errors.Select(e => e.Description))));

        return Ok(ApiResponse.Ok<object?>(null, "Role created"));
    }

    [HttpDelete("{name}")]
    [HasPermission(Permissions.RolesDelete)]
    public async Task<IActionResult> Delete(string name)
    {
        var role = await roleManager.FindByNameAsync(name);
        if (role is null) return NotFound(ApiResponse.Fail("Role not found"));

        await roleManager.DeleteAsync(role);
        return Ok(ApiResponse.Ok<object?>(null, "Role deleted"));
    }

    /// <summary>Replace a role's permission set entirely.</summary>
    [HttpPost("{name}/permissions")]
    [HasPermission(Permissions.RolesEdit)]
    public async Task<IActionResult> AssignPermissions(
        string name, [FromBody] AssignPermissionsRequest req)
    {
        var role = await roleManager.FindByNameAsync(name);
        if (role is null) return NotFound(ApiResponse.Fail("Role not found"));

        // Remove existing permission claims
        var existing = await roleManager.GetClaimsAsync(role);
        foreach (var claim in existing.Where(c => c.Type == "Permission"))
            await roleManager.RemoveClaimAsync(role, claim);

        // Add new ones
        foreach (var permission in req.Permissions)
            await roleManager.AddClaimAsync(role, new Claim("Permission", permission));

        return Ok(ApiResponse.Ok<object?>(null, "Permissions updated"));
    }

    /// <summary>Lists all permission strings defined in the system.</summary>
    [HttpGet("permissions/all")]
    [HasPermission(Permissions.RolesView)]
    public IActionResult GetAllPermissions() =>
        Ok(ApiResponse.Ok(Permissions.GetAll()));
}
