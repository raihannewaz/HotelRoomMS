using Common.Core.Query;
using Common.Identity.Roles.Dtos;

namespace Common.Identity.Roles.Features.GettingRoles;

public record GetRolesResponse(ListResultModel<RoleDto> Roles);
public record GettingRolesResponse(List<RoleDto> Roles);
