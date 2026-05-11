using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Exceptions;
using Common.Identity.Shared;
using Common.Identity.Shared.Models;
using Common.Identity.Roles.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Common.Identity.Roles.Features.UpdateRoleClaims;

public record UpdateRoleClaim(UpdateRoleClaimsRequest Update) : IRequest<UpdateRoleClaimsResponse>
{
}

internal class AddRoleClaimValidator : AbstractValidator<UpdateRoleClaim>
{
    public AddRoleClaimValidator()
    {

        RuleFor(v => v.Update.Id)
            .NotEmpty()
            .NotNull()
            .GreaterThan(0);

        RuleFor(v => v.Update.Permissions).NotNull().WithMessage("Permission is reqiured.");
    }
}

internal class AddRoleClaimHandler : IRequestHandler<UpdateRoleClaim, UpdateRoleClaimsResponse>
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

    public async Task<UpdateRoleClaimsResponse> Handle(UpdateRoleClaim request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        //if (Convert.ToInt64(_securityContextAccessor.UserId) > 1 && !_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.ROLEEDIT, StringComparer.Ordinal))
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

        using var conn = _connectionFactory.GetOrCreateConnection();

        var role = await _roleManager.FindByIdAsync(request.Update.Id.ToString()) ?? throw new ApiException("Role not found.", System.Net.HttpStatusCode.NotFound);

        Guard.Against.NotFound(!request.Update.Permissions.Any(), new ApiException("At least one permission is required."));

        bool hasDuplicates = request.Update.Permissions.Count() != request.Update.Permissions.Distinct(StringComparer.OrdinalIgnoreCase).Count();

        if (hasDuplicates)
        {
           throw new ApiException("Duplicate Permissions found.");
        }

        var allClaims = await _roleManager.GetClaimsAsync(role);
        //foreach (var claim in allClaims)
        //    await _roleManager.RemoveClaimAsync(role, claim);

        foreach (var item in request.Update.Permissions)
        {

            if(!allClaims.Any(x => x.Value == item))
            {
                await _roleManager.AddClaimAsync(role, new Claim(item, item.ToUpper()));
            }
           
            //if (!addClaim.Succeeded)
            //{
            //    using var transaction = conn.BeginTransaction();
            //    const string delete_role_claims = "DELETE FROM [users].[asp_net_role_claims] where role_id = @id";
            //    await conn.ExecuteAsync(delete_role_claims, new { id = request.Id }, transaction: transaction);
            //    transaction.Commit();
            //    throw new RegisterIdentityUserException(string.Join(',', addClaim.Errors.Select(e => e.Description)));
            //}
        }

        foreach (var claim in allClaims)
        {

            if (!request.Update.Permissions.Any(x => x == claim.Value))
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            //if (!addClaim.Succeeded)
            //{
            //    using var transaction = conn.BeginTransaction();
            //    const string delete_role_claims = "DELETE FROM [users].[asp_net_role_claims] where role_id = @id";
            //    await conn.ExecuteAsync(delete_role_claims, new { id = request.Id }, transaction: transaction);
            //    transaction.Commit();
            //    throw new RegisterIdentityUserException(string.Join(',', addClaim.Errors.Select(e => e.Description)));
            //}
        }

        return new UpdateRoleClaimsResponse(new RoleClaimsDto
        {
            Id = request.Update.Id,
            Name = role.Name,
            Permissions = request.Update.Permissions,
        });
    }
}
