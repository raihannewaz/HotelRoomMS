using Common.Identity.Shared.Models;
using Common.Identity.Roles.Dtos;
using Common.Identity.Roles.Features.GettingRoles;

namespace Common.Identity.Roles;

public static class RolesMapper
{

    public static RolePermissionDto QueryResponse(ApplicationRole entity)
    {
        return new RolePermissionDto
        {
            Id = entity.Id,
            Name = entity.Name,
        };
    }


    public static GetRolesGrid GetRequestMap(GetRolesRequest request)
    {
        return new GetRolesGrid()
        {
            Includes = request.Includes,
            Filters = request.Filters,
            Sorts = request.Sorts,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
