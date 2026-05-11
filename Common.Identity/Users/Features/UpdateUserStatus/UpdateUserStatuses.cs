using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Command;
using BuildingBlocks.Core.Exception;
using BuildingBlocks.Core.Exception.Types;
using BuildingBlocks.Security.Jwt;
using FluentValidation;
using Common.Identity.Shared;
using Common.Identity.Shared.Data;
using Common.Identity.Shared.Exceptions;
using Common.Identity.Shared.Models;
using Common.Identity.Identity.Users.Features.RegisteringUser;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Identity.Users.Features.UpdateUserStatus;

public record UpdateUserStatuses(long Id) : ITxUpdateCommand<UpdateUserStatusResponse>
{
}

internal class UpdateUserValidator : AbstractValidator<UpdateUserStatuses>
{
    public UpdateUserValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(v => v.Id).NotNull().GreaterThan(0);
    }
}

internal class UpdateUserHandler : ICommandHandler<UpdateUserStatuses, UpdateUserStatusResponse>
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

    public async Task<UpdateUserStatusResponse> Handle(UpdateUserStatuses request, CancellationToken cancellationToken)
    {
        if (!_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.USEREDIT, StringComparer.Ordinal))
            throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        Guard.Against.Null(user, new UserNotFoundException(request.Id));

        string status = "Active";
        if (user?.UserState == UserState.Active)
        {
            user.UserState = UserState.InActive;
            status = "InActive";
        }
        else if (user?.UserState == UserState.InActive || user?.UserState == UserState.Locked)
        {
            user.UserState = UserState.Active;
            status = "Active";
        }

        var _data = await _userManager.UpdateAsync(user);

        if (!_data.Succeeded)
            throw new RegisterIdentityUserException(string.Join(',', _data.Errors.Select(e => e.Description)));

        return new UpdateUserStatusResponse(request.Id, status);
    }
}
