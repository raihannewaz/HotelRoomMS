using Common.Identity.Roles.Dtos;

namespace Common.Identity.Roles.Features.GetAllPermissions;

public record GetPermissionResponse(IEnumerable<PermissionDto> List);
