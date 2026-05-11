using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.Exceptions;
using Common.Identity.Shared;
using Common.Identity.Shared.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Users.Features.GettingUerByEmail;

public record GetUserByEmail(string Email) : IRequest<GetUserByEmailResponse>;

internal class GetUserByIdValidator : AbstractValidator<GetUserByEmail>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email address is not valid");
    }
}

internal class GetUserByEmailHandler : IRequestHandler<GetUserByEmail, GetUserByEmailResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public GetUserByEmailHandler(UserManager<ApplicationUser> userManager, ISecurityContextAccessor securityContextAccessor)
    {
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<GetUserByEmailResponse> Handle(GetUserByEmail query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        //if (!_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.USERVIEW, StringComparer.Ordinal))
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

        var identityUser = await _userManager.FindByEmailAsync(query.Email);

        var userDto = UsersMapper.QueryResponse(identityUser);

        return new GetUserByEmailResponse(userDto);
    }
}
