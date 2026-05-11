using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.Exceptions;
using Common.Identity.Identity;
using Common.Identity.Shared;
using Common.Identity.Shared.Data;
using Common.Identity.Shared.Models;
using Common.Identity.Users.Dtos;
using Common.Identity.Users.Features.RegisteringUser;
using Common.Identity.Users.Features.UpdateUser.Request;
using Common.Identity.Users.Features.UpdateUser.Response;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Common.Identity.Users.Features.UpdateUser;

public record UpdateUser(UpdateUserRequest updateRequest) : IRequest<UpdateUserResponse>
{
}

internal class UpdateUserValidator : AbstractValidator<UpdateUser>
{
    public UpdateUserValidator()
    {

        RuleFor(v => v.updateRequest.Id).NotNull().GreaterThan(0);

        RuleFor(v => v.updateRequest.FirstName)
            .NotEmpty()
            .WithMessage("FirstName is required.");

        RuleFor(v => v.updateRequest.LastName)
            .NotEmpty()
            .WithMessage("LastName is required.");

        RuleFor(v => v.updateRequest.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress();

        RuleFor(v => v.updateRequest.UserName)
            .NotEmpty()
            .WithMessage("UserName is required.");

        RuleFor(v => v.updateRequest.ActionType)
            .NotEmpty()
            .WithMessage("Action Type is required.");

        When(customer => string.Equals(customer.updateRequest.ActionType, "Password", StringComparison.Ordinal), () =>
        {
            RuleFor(v => v.updateRequest.Password).NotEmpty().NotNull().WithMessage("Password is required.");
            RuleFor(v => v.updateRequest.ConfirmPassword).Equal(x => x.updateRequest.Password).WithMessage("The password and confirmation password do not match.")
                .NotEmpty().NotNull();
        });

        RuleFor(v => v.updateRequest.Roles).NotNull().WithMessage("Role is reqiured.");

        //RuleFor(v => v.Roles).Custom((roles, c) =>
        //{
        //    if (roles != null &&
        //        !roles.All(x => x.Contains(Constants.Role.Admin, StringComparison.Ordinal) ||
        //                        x.Contains(Constants.Role.User, StringComparison.Ordinal) ||
        //                        x.Contains(Constants.Role.Guest, StringComparison.Ordinal) ||
        //                        x.Contains(Constants.Role.Manager, StringComparison.Ordinal)))
        //    {
        //        c.AddFailure("Invalid roles.");
        //    }
        //});
    }
}

internal class UpdateUserHandler : IRequestHandler<UpdateUser, UpdateUserResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentityContext _context;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public UpdateUserHandler(UserManager<ApplicationUser> userManager, IdentityContext context, ISecurityContextAccessor securityContextAccessor)
    {
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
        _context = context;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<UpdateUserResponse> Handle(UpdateUser request, CancellationToken cancellationToken)
    {
        //var tenantId = request.TenantId;

        //if (_securityContextAccessor.IsAuthenticated)
        //{
        //    tenantId = await _context.Tenants.Where(t => t.ApiKey == _securityContextAccessor.ApiKey)
        //        .Select(t => t.Name).FirstOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        //}

        if (_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.USEREDIT, StringComparer.Ordinal) ||
            _securityContextAccessor.Permissions.Contains(IdentityModulePermissions.USERPROFILEVIEW, StringComparer.Ordinal) ||
            _securityContextAccessor.Permissions.Contains(IdentityModulePermissions.USERPROFILEEDIT, StringComparer.Ordinal))
        {
            

            var user = await _userManager.FindByIdAsync(request.updateRequest.Id.ToString());
            if (user == null)
                throw new ApiException($"User Not Found With Name {request.updateRequest.UserName}", HttpStatusCode.NotFound);


            var isEmailExist = _context.Users.Any(u => u.Email == request.updateRequest.Email && u.Id != request.updateRequest.Id);
            if (isEmailExist == true)
                throw new ApiException("Email Already Exist", HttpStatusCode.BadRequest);
       

            var isUsernameExist = _context.Users.Any(u => u.UserName == request.updateRequest.UserName && u.Id != request.updateRequest.Id);
            if (isUsernameExist == true)
                throw new ApiException("Username Already Exist", HttpStatusCode.BadRequest);


            user.FirstName = request.updateRequest.FirstName.Trim();
            user.LastName = request.updateRequest.LastName.Trim();
            user.UserName = request.updateRequest.UserName.Trim();
            user.Email = request.updateRequest.Email.Trim();
            user.UserState = UserState.Active;

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles.ToArray());

            var roleResult = await _userManager.AddToRolesAsync(
                user,
                request.updateRequest.Roles ?? new List<string> { Constants.Role.User });

            if (!roleResult.Succeeded)
                throw new RegisterIdentityUserException(string.Join(',', roleResult.Errors.Select(e => e.Description)));

            var _data = await _userManager.UpdateAsync(user);

            if (string.Equals(request.updateRequest.ActionType, "Password", StringComparison.Ordinal) && _data.Succeeded)
            {
                var removePass = await _userManager.RemovePasswordAsync(user);
                if (removePass.Succeeded)
                    await _userManager.AddPasswordAsync(user, request.updateRequest.Password);
            }

            return new UpdateUserResponse(new IdentityUserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = request.updateRequest.Roles ?? new List<string> { Constants.Role.User },
                RefreshTokens = user?.RefreshTokens?.Select(x => x.Token),
                UserState = UserState.Active,
            });
        }
        else
        {
            throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);
        }
    }
}
