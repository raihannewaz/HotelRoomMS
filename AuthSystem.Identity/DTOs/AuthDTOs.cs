using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Identity.DTOs;

// ── Auth ───────────────────────────────────────────────────────────────────────

public record RegisterRequest(
    [Required] string FullName,
    [Required, EmailAddress] string Email,
    [Required, MaxLength(100)] string UserName,
    [Required, MinLength(6)] string Password
);

public record LoginRequest(
    [Required] string EmailOrUsreName,
    [Required] string Password
);

public record AuthResponse(
    string Token,
    string Email,
    string FullName,
    IEnumerable<string> Roles,
    IEnumerable<string> Permissions,
    DateTime ExpiresAt
);

// ── Users ──────────────────────────────────────────────────────────────────────

public record UserDto(
    string Id,
    string FullName,
    string Email,
    bool IsActive,
    DateTime CreatedAt,
    IEnumerable<string> Roles
);

public record UpdateUserRequest(
    string? FullName,
    bool? IsActive
);

public record AssignRolesRequest(
    [Required] IEnumerable<string> Roles
);

// ── Roles ──────────────────────────────────────────────────────────────────────

public record CreateRoleRequest(
    [Required] string Name
);

public record RoleDto(
    string Id,
    string Name,
    IEnumerable<string> Permissions
);

public record AssignPermissionsRequest(
    [Required] IEnumerable<string> Permissions
);

// ── Generic response wrapper ───────────────────────────────────────────────────

public record ApiResponse<T>(bool Success, string? Message, T? Data);

public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(T data, string? msg = null) => new(true, msg, data);
    public static ApiResponse<object> Fail(string msg) => new(false, msg, null);
}
