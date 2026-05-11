using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Abstractions.Messaging;
using Common.Core.DateTimeConversions;
using Common.Core.IdsGenerator;
using Common.Identity.Identity;
using Common.Identity.Shared.Data;
using Common.Identity.Shared.Models;
using Common.Identity.Users.Dtos;
using Common.Identity.Users.Features.RegisteringUser.Events.Integration;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using UserState = Common.Identity.Shared.Models.UserState;

namespace Common.Identity.Users.Features.RegisteringUser;

public record RegisterUser(
RegisterUserRequest UserRequest) : IRequest<RegisterUserResponse>
{
    public DateTime CreatedAt { get; init; } = DateTimeConversion.UTCToBST();
    //public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}

internal class RegisterUserValidator : AbstractValidator<RegisterUser>
{
    public RegisterUserValidator()
    {


        RuleFor(v => v.UserRequest.FirstName)
            .NotEmpty()
            .WithMessage("FirstName is required.");

        RuleFor(v => v.UserRequest.LastName)
            .NotEmpty()
            .WithMessage("LastName is required.");

        RuleFor(v => v.UserRequest.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress();

        RuleFor(v => v.UserRequest.UserName)
            .NotEmpty()
            .WithMessage("UserName is required.");

        RuleFor(v => v.UserRequest.Password)
            .NotEmpty()
            .WithMessage("Password is required.");

        RuleFor(v => v.UserRequest.ConfirmPassword)
            .Equal(x => x.UserRequest.Password)
            .WithMessage("The password and confirmation password do not match.")
            .NotEmpty();

        RuleFor(v => v.UserRequest.Roles).NotNull().WithMessage("Role is reqiured.");

        //RuleFor(v => v.Roles).Custom((roles, c) =>
        //{
        //    if (roles != null)
        //        // && !roles.All(x => x.Contains(Constants.Role.Admin, StringComparison.Ordinal) ||
        //        //                x.Contains(Constants.Role.User, StringComparison.Ordinal) ||
        //        //                x.Contains(Constants.Role.Manager, StringComparison.Ordinal) ||
        //        //                x.Contains(Constants.Role.Guest, StringComparison.Ordinal)))
        //    {
        //        c.AddFailure("Invalid roles.");
        //    }
        //});
    }
}

internal class RegisterUserHandler : IRequestHandler<RegisterUser, RegisterUserResponse>
{
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IdentityContext _context;
    private readonly IBus _bus;
    private readonly IDbConnectionCreator _connectionFactory;
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterUserHandler(
        UserManager<ApplicationUser> userManager,
        ISecurityContextAccessor securityContextAccessor,
        IdentityContext context,
        IBus bus,
        IDbConnectionCreator connectionFactory)
    {
        _securityContextAccessor = securityContextAccessor;
        _context = context;
        _bus = bus;
        _connectionFactory = connectionFactory;
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
    }

    public async Task<RegisterUserResponse> Handle(RegisterUser request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.UserRequest.TenantId, "Tenant Id is required.");

        //if (request.UserRequest.TenantId != "eshop" &&
        //    !_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.USERCREATE, StringComparer.Ordinal))
        //{
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);
        //}

        using var con = _connectionFactory.GetOrCreateConnection();
        const string primary_key_sql = "SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END FROM [users].[asp_net_users] where id = @id;";
        long PrimaryId = CurrentDateTimeCountIdGenerator.Id();
        long incrementalValue = 1;
        var isPKExists = await con.QueryFirstOrDefaultAsync<bool>(primary_key_sql, new { id = PrimaryId });
        while (isPKExists)
        {
            PrimaryId += incrementalValue;
            isPKExists = await con.QueryFirstOrDefaultAsync<bool>(primary_key_sql, new { id = PrimaryId });
            incrementalValue++;
        }

        var applicationUser = new ApplicationUser
        {
            //Id = PrimaryId,
            FirstName = request.UserRequest.FirstName.Trim(),
            LastName = request.UserRequest.LastName.Trim(),
            UserName = request.UserRequest.UserName.Trim(),
            Email = request.UserRequest.Email.Trim(),
            UserState = UserState.Active,
            CreatedAt = request.CreatedAt,
        };

        var identityResult = await _userManager.CreateAsync(applicationUser, request.UserRequest.Password);
        var roleResult = await _userManager.AddToRolesAsync(
            applicationUser,
            request.UserRequest.Roles ?? new List<string> { Constants.Role.User });

        if (!identityResult.Succeeded)
            throw new RegisterIdentityUserException(string.Join(',', identityResult.Errors.Select(e => e.Description)));

        if (!roleResult.Succeeded)
            throw new RegisterIdentityUserException(string.Join(',', roleResult.Errors.Select(e => e.Description)));

        await _bus.PublishAsync(
            new UserRegistered(
                applicationUser.Id,
                applicationUser.Email,
                applicationUser.UserName,
                applicationUser.FirstName,
                applicationUser.LastName,
                request.UserRequest.Roles?.ToList()),
            null,
            cancellationToken);

        return new RegisterUserResponse(new IdentityUserDto
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            UserName = applicationUser.UserName,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            Roles = request.UserRequest.Roles ?? new List<string> { Constants.Role.User },
            RefreshTokens = applicationUser?.RefreshTokens?.Select(x => x.Token),
            CreatedAt = request.CreatedAt,
            UserState = UserState.Active
        });
    }
}
