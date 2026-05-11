using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Exceptions;
using Common.Identity.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Roles.Features.GetAllRolesForUser;

public record GettingAllRolesForUser() : IRequest<GetAllRolesForUserRespose>;

public class GroupTreeHandler : IRequestHandler<GettingAllRolesForUser, GetAllRolesForUserRespose>
{
    private readonly IDbConnectionCreator _connectionFactory;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public GroupTreeHandler(
        IDbConnectionCreator connectionFactory,
        ISecurityContextAccessor securityContextAccessor,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _connectionFactory = connectionFactory;
        _securityContextAccessor = securityContextAccessor;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<GetAllRolesForUserRespose> Handle(GettingAllRolesForUser request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var userId = Convert.ToInt64(_securityContextAccessor.UserId);
        //if (userId == 0)
        //    throw new ApiException("User not found");

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user?.Id == null)
            throw new ApiException("User not found");

        var roles = await _userManager.GetRolesAsync(user);

        string permissions = string.Empty;

        if (roles.Count > 0)
        {
            foreach (var role in roles)
            {
                var roleObject = await _roleManager.FindByNameAsync(role);
                var permission = await _roleManager.GetClaimsAsync(roleObject);
                permissions += ", " + string.Join(", ", permission.Select(s => s.Type));
            }
        }

        List<string> listOfPermissions = [];

        if (permissions.Length > 0)
            listOfPermissions = new(permissions[1..].Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).OrderBy(s => s));

        return new GetAllRolesForUserRespose(listOfPermissions);
    }
}
