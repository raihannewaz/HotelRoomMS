using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Exceptions;
using Common.Identity.Shared;
using Common.Identity.Shared.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Roles.Features.GetRoleById;

public record GetRoleById(long Id) : IRequest<UserRoleByIdResponse>;

internal class GetRoleByIdValidator : AbstractValidator<GetRoleById>
{
    public GetRoleByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}

internal class GetRoleByIdHandler : IRequestHandler<GetRoleById, UserRoleByIdResponse>
{

    private readonly IDbConnectionCreator _connectionFactory;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public GetRoleByIdHandler(IDbConnectionCreator connectionFactory, RoleManager<ApplicationRole> roleManager,
        ISecurityContextAccessor securityContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _roleManager = roleManager;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<UserRoleByIdResponse> Handle(GetRoleById query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        //if (Convert.ToInt64(_securityContextAccessor.UserId) > 1 && !_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.ROLEVIEW, StringComparer.Ordinal))
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

        var role = await _roleManager.FindByIdAsync(query.Id.ToString());
        if (role == null)
            throw new ApiException("Role not found with id " + query.Id, System.Net.HttpStatusCode.NotFound);

        var roleDto = RolesMapper.QueryResponse(role);

        var claims = await _roleManager.GetClaimsAsync(role);

        roleDto.Permissions = claims.Select(s => s.Type);

        return new UserRoleByIdResponse(roleDto);
    }
}
