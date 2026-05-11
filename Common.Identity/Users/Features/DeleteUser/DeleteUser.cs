using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.Exceptions;
using Common.Identity.Identity.Exceptions;
using Common.Identity.Shared;
using Common.Identity.Shared.Exceptions;
using Common.Identity.Shared.Models;
using Common.Identity.Users.Features.DeleteUser;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Users.Features.DeleteUser;

public record DeleteUser(long Id) : IRequest<DeleteUserResponse>
{ }

internal class DeleteUserValidator : AbstractValidator<DeleteUser>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}

internal class DeleteUserHandler : IRequestHandler<DeleteUser, DeleteUserResponse>
{

    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserHandler(UserManager<ApplicationUser> userManager, ISecurityContextAccessor securityContextAccessor)
    {
        _securityContextAccessor = securityContextAccessor;
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
    }

    public async Task<DeleteUserResponse> Handle(DeleteUser request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        //if (!_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.USERDELETE, StringComparer.Ordinal))
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

        var identityUser = await _userManager.FindByIdAsync(request.Id.ToString());
        if (identityUser == null)
        {
            throw new UserNotFoundException(request.Id);
        }
         

        identityUser.UserState = UserState.Deleted;

        var reponse = await _userManager.UpdateAsync(identityUser);
      //  var reponse = await _userManager.DeleteAsync(identityUser);
        if (!reponse.Succeeded)
        {
            var message = reponse.Errors.Select(s => s.Description).ToList();
            throw new UserCannotDeleteException(string.Join(", ", message));
        }

        return new DeleteUserResponse(request.Id);
    }
}
