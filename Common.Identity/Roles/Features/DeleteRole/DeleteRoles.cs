using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.Exceptions;
using Common.Identity.Shared;
using Common.Identity.Shared.Models;
using Common.Identity.Users.Features.RegisteringUser;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Roles.Features.DeleteRole;

public record DeleteRoles(long Id) : IRequest<DeleteRoleResponse>
{
}

internal class UpdateRolesValidator : AbstractValidator<DeleteRoles>
{
    public UpdateRolesValidator()
    {


        RuleFor(v => v.Id).NotNull().GreaterThan(0);
    }
}

internal class UpdateRolesHandler : IRequestHandler<DeleteRoles, DeleteRoleResponse>
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public UpdateRolesHandler(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ISecurityContextAccessor securityContextAccessor
        )
    {
        _roleManager = Guard.Against.Null(roleManager, nameof(roleManager));
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<DeleteRoleResponse> Handle(DeleteRoles request, CancellationToken cancellationToken)
    {
        //if (Convert.ToInt64(_securityContextAccessor.UserId) > 1 && !_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.ROLEDELETE, StringComparer.Ordinal))
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

        var role = await _roleManager.FindByIdAsync(request.Id.ToString()) ??
                        throw new ApiException("Role not found.", System.Net.HttpStatusCode.NotFound);

        var roleUsers = await _userManager.GetUsersInRoleAsync(role.Name);
        if (roleUsers.Count <= 0)
        {
            var identityRole = await _roleManager.DeleteAsync(role);

            if (!identityRole.Succeeded)
                throw new RegisterIdentityUserException(string.Join(',', identityRole.Errors.Select(e => e.Description)));

            return new DeleteRoleResponse();
        }

        throw new ApiException("Role cannot be deleted.", System.Net.HttpStatusCode.Conflict);
    }
}
