
using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Exceptions;
using Common.Identity.Roles.Dtos;
using Common.Identity.Shared;
using Common.Identity.Shared.Models;
using Common.Identity.Users.Features.RegisteringUser;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Common.Identity.Roles.Features.AddRoleClaims;

public record AddRoleClaim(AddRoleClaimsRequest RoleClaim) : IRequest<AddRoleClaimsResponse>
{
}

internal class AddRoleClaimValidator : AbstractValidator<AddRoleClaim>
{
    public AddRoleClaimValidator()
    {

        RuleFor(v => v.RoleClaim.Id)
            .NotEmpty()
            .NotNull()
            .GreaterThan(0);

        RuleFor(v => v.RoleClaim.Permissions).NotNull().WithMessage("Permission is reqiured.");
    }
}

internal class AddRoleClaimHandler : IRequestHandler<AddRoleClaim, AddRoleClaimsResponse>
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IDbConnectionCreator _connectionFactory;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public AddRoleClaimHandler(RoleManager<ApplicationRole> roleManager, IDbConnectionCreator connectionFactory, ISecurityContextAccessor securityContextAccessor)
    {
        _roleManager = roleManager;
        _connectionFactory = connectionFactory;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<AddRoleClaimsResponse> Handle(AddRoleClaim request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        //if (Convert.ToInt64(_securityContextAccessor.UserId) > 1 && !_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.ROLECREATE, StringComparer.Ordinal))
        //{
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);
        //}

        using var conn = _connectionFactory.GetOrCreateConnection();

        var role = await _roleManager.FindByIdAsync(request.RoleClaim.Id.ToString()) ?? throw new ApiException("Role not found.", System.Net.HttpStatusCode.NotFound);

        Guard.Against.NotFound(!request.RoleClaim.Permissions.Any(), new ApiException("At least one permission is required."));

        bool hasDuplicates = request.RoleClaim.Permissions.Count() != request.RoleClaim.Permissions.Distinct(StringComparer.OrdinalIgnoreCase).Count();

        if (hasDuplicates)
        {
            new ApiException("Duplicate Permissions found.");
        }


        foreach (var item in request.RoleClaim.Permissions)
        {
            var claims = new Claim(item, item.ToUpper());
            var addClaim = await _roleManager.AddClaimAsync(role, claims);
            if (!addClaim.Succeeded)
            {
                using var transaction = conn.BeginTransaction();
                const string delete_role_claims = "DELETE FROM [users].[asp_net_role_claims] where role_id = @id";
                await conn.ExecuteAsync(delete_role_claims, new { id = request.RoleClaim.Id }, transaction: transaction);
                transaction.Commit();
                throw new RegisterIdentityUserException(string.Join(',', addClaim.Errors.Select(e => e.Description)));
            }
        }

        return new AddRoleClaimsResponse(new RoleClaimsDto
        {
            Id = request.RoleClaim.Id,
            Name = role.Name,
            Permissions = request.RoleClaim.Permissions,
        });
    }
}
