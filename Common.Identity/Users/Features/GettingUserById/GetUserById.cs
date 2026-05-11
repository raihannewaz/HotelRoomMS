using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Exceptions;
using Common.Identity.Shared;
using Common.Identity.Shared.Exceptions;
using Common.Identity.Shared.Models;
using Common.Identity.Users;
using Common.Identity.Users.Dtos;
using Common.Identity.Users.Features.GettingUserById;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Common.Identity.Identity.Users.Features.GettingUserById;

public record GetUserById(long Id) : IRequest<UserByIdResponse>;

internal class GetUserByIdValidator : AbstractValidator<GetUserById>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}

internal class GetUserByIdHandler : IRequestHandler<GetUserById, UserByIdResponse>
{
    private readonly IDbConnectionCreator _connectionFactory;
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdHandler(UserManager<ApplicationUser> userManager, IDbConnectionCreator connectionFactory, ISecurityContextAccessor securityContextAccessor)
    {
        _connectionFactory = connectionFactory;
        _securityContextAccessor = securityContextAccessor;
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
    }

    public async Task<UserByIdResponse> Handle(GetUserById query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        //if (!_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.USERVIEW, StringComparer.Ordinal) ||
        //    !_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.USERPROFILEVIEW, StringComparer.Ordinal) ||
        //    !_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.USERPROFILEEDIT, StringComparer.Ordinal))
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

        var identityUser = await _userManager.FindByIdAsync(query.Id.ToString());
        if (identityUser == null)
            throw new ApiException($"No User Found{query.Id.ToString()}", HttpStatusCode.NotFound);


        var identityUserDto = UsersMapper.QueryResponse(identityUser);

        identityUserDto.Roles = await _userManager.GetRolesAsync(identityUser);

        using var con = _connectionFactory.GetOrCreateConnection();


        return new UserByIdResponse(identityUserDto);
    }
}
