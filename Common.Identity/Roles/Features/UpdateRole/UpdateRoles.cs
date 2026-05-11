using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Abstractions.Messaging;
using Common.Core.Exceptions;
using Common.Identity.Shared;
using Common.Identity.Shared.Data;
using Common.Identity.Shared.Models;
using Common.Identity.Users.Features.RegisteringUser;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Roles.Features.UpdateRole;

public record UpdateRoles(UpdateRoleRequest update) : IRequest<UpdateRoleResponse>
{
}

internal class UpdateRolesValidator : AbstractValidator<UpdateRoles>
{
    public UpdateRolesValidator()
    {

        RuleFor(v => v.update.Id).NotNull().GreaterThan(0);

        RuleFor(v => v.update.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}

internal class UpdateRolesHandler : IRequestHandler<UpdateRoles, UpdateRoleResponse>
{
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IdentityContext _context;
    private readonly IBus _bus;
    private readonly IDbConnectionCreator _connectionFactory;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UpdateRolesHandler(
        RoleManager<ApplicationRole> roleManager,
        ISecurityContextAccessor securityContextAccessor,
        IdentityContext context,
        IBus bus,
        IDbConnectionCreator connectionFactory)
    {
        _securityContextAccessor = securityContextAccessor;
        _context = context;
        _bus = bus;
        _connectionFactory = connectionFactory;
        _roleManager = Guard.Against.Null(roleManager, nameof(roleManager));
    }

    public async Task<UpdateRoleResponse> Handle(UpdateRoles request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        //if (!_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.ROLEEDIT, StringComparer.Ordinal))
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

        using var con = _connectionFactory.GetOrCreateConnection();

        var userId = Convert.ToInt64(_securityContextAccessor.UserId);

        var tenatId = _securityContextAccessor.TenantId;
        if (string.IsNullOrEmpty(tenatId))
            tenatId = await con.QueryFirstOrDefaultAsync<string>($"SELECT tenant_id from [users].[asp_net_users] where id = @userId;", new { userId });
        if (string.IsNullOrEmpty(tenatId))
            tenatId = "Main";

        var role = await _roleManager.FindByIdAsync(request.update.Id.ToString());
        if (role == null)
            throw new ApiException("Role not found.", System.Net.HttpStatusCode.NotFound);

        role.Name = request.update.Name.Trim();

        var identityRole = await _roleManager.UpdateAsync(role);

        if (!identityRole.Succeeded)
            throw new RegisterIdentityUserException(string.Join(',', identityRole.Errors.Select(e => e.Description)));

        return new UpdateRoleResponse(request.update.Id);
    }
}
