using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Identity.Roles.Data;

namespace Common.Identity.Roles.Features.GetAllPermissions;

public record GetPermissionList() : IRequest<GetPermissionResponse>;

public class GroupTreeHandler : IRequestHandler<GetPermissionList, GetPermissionResponse>
{
    public async Task<GetPermissionResponse> Handle(GetPermissionList request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        return new GetPermissionResponse(AdminPermission.ListOfPermissions
                                                    .OrderBy(s => s.Module)
                                                    .ThenBy(s => s.SubModule)
                                                    .ThenBy(s => s.Text));
    }
}
