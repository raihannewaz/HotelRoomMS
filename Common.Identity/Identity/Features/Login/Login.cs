using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.Jwt;
using Common.Identity.Identity.Exceptions;
using Common.Identity.Shared.Exceptions;
using Common.Identity.Shared.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Common.Identity.Identity.Features.Login;

public record Login(string UserNameOrEmail, string Password, bool Remember) :
    IRequest<LoginResponse>;

internal class LoginValidator : AbstractValidator<Login>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty().WithMessage("UserNameOrEmail cannot be empty");
        RuleFor(x => x.Password).NotEmpty().WithMessage("password cannot be empty");
    }
}

internal class LoginHandler : IRequestHandler<Login, LoginResponse>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IJwtService _jwtService;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<LoginHandler> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginHandler(
        UserManager<ApplicationUser> userManager,
        ICommandProcessor commandProcessor,
        IJwtService jwtService,
        IOptions<JwtOptions> jwtOptions,
        SignInManager<ApplicationUser> signInManager,
        ILogger<LoginHandler> logger)
    {
        _userManager = userManager;
        _commandProcessor = commandProcessor;
        _jwtService = jwtService;
        _signInManager = signInManager;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(Login request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(Login));

        var identityUser = await _userManager.FindByNameAsync(request.UserNameOrEmail) ??
                           await _userManager.FindByEmailAsync(request.UserNameOrEmail);

        if (identityUser == null)
        {
            throw new UserNotFoundException(request.UserNameOrEmail);
        } 

        if (identityUser?.UserState == UserState.InActive || identityUser?.UserState == UserState.Deleted)
            throw new UserStateException("Please contact Admin.");

        var signinResult = await _signInManager.CheckPasswordSignInAsync(identityUser, request.Password, request.Remember);

        if (signinResult.IsNotAllowed)
        {
            if (!await _userManager.IsEmailConfirmedAsync(identityUser))
                throw new EmailNotConfirmedException(identityUser.Email);

            if (!await _userManager.IsPhoneNumberConfirmedAsync(identityUser))
                throw new PhoneNumberNotConfirmedException(identityUser.PhoneNumber);
        }
        else if (signinResult.IsLockedOut)
        {
            throw new UserLockedException(identityUser.Id.ToString());
        }
        else if (signinResult.RequiresTwoFactor)
        {
            throw new RequiresTwoFactorException("Require two factor authentication.");
        }
        else if (!signinResult.Succeeded)
        {
            throw new PasswordIsInvalidException("Password is invalid.");
        }

        var refreshToken =
            (await _commandProcessor.SendAsync(
                new GenerateRefreshToken.GenerateRefreshToken { UserId = identityUser.Id },
                cancellationToken)).RefreshToken;

        var accessToken =
            await _commandProcessor.SendAsync(
                new GenerateJwtToken.GenerateJwtToken(identityUser, refreshToken.Token),
                cancellationToken);

        _logger.LogInformation("User with ID: {ID} has been authenticated", identityUser.Id);

        return new LoginResponse(identityUser, accessToken, refreshToken.Token);
    }
}
