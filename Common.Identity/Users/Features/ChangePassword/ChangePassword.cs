using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Identity.Shared.Data;
using Common.Identity.Shared.Models;
using Common.Identity.Users.Features.ChangePassword.Request;
using Common.Identity.Users.Features.ChangePassword.Response;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Users.Features.ChangePassword;

public record ChangePassword(ChangePasswordRequest Change) : IRequest<ChangePasswordResponse>;

internal class ChangePasswordValidator : AbstractValidator<ChangePassword>
{
    public ChangePasswordValidator()
    {

        RuleFor(v => v.Change.Id).NotEmpty().NotNull().WithMessage("User id is required.");
        RuleFor(v => v.Change.OldPassword).NotEmpty().NotNull().WithMessage("Old Password is required.");
        RuleFor(v => v.Change.NewPassword).NotEmpty().NotNull().WithMessage("New Password is required.");
    }
}

internal class ChangePasswordHandler : IRequestHandler<ChangePassword, ChangePasswordResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentityContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public ChangePasswordHandler(UserManager<ApplicationUser> userManager, IdentityContext context,ISecurityContextAccessor securityContextAccessor)
    {
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<ChangePasswordResponse> Handle(ChangePassword request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        //if (!_securityContextAccessor.Permissions.Contains(APPModulePermissions.BRANCHCREATE, StringComparer.Ordinal))
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

        var identityUser = await _userManager.FindByIdAsync(request.Change.Id.ToString());
        if (identityUser == null)
            Guard.Against.Null(identityUser, "No user found with the User Id " + request.Change.Id.ToString());

        var result = await _userManager.ChangePasswordAsync(identityUser, request.Change.OldPassword, request.Change.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var item in result.Errors)
            {
                throw new ArgumentException(message: item.Description);
            }
        }

        return new ChangePasswordResponse();
    }
}
