using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Identity.Shared.Exceptions;
using Common.Identity.Shared.Models;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Identity.Features.RefreshingToken;

public record RefreshToken(RefreshTokenRequest Token) : IRequest<RefreshTokenResponse>;

internal class RefreshTokenValidator : AbstractValidator<RefreshToken>
{
    public RefreshTokenValidator()
    {
        RuleFor(v => v.Token.AccessToken)
            .NotEmpty();

        RuleFor(v => v.Token.RefreshToken)
            .NotEmpty();
    }
}

internal class RefreshTokenHandler : IRequestHandler<RefreshToken, RefreshTokenResponse>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IDbConnectionCreator _connectionFactory;
    private readonly IJwtService _jwtService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RefreshTokenHandler(
        IJwtService jwtService,
        UserManager<ApplicationUser> userManager,
        ICommandProcessor commandProcessor,
        IDbConnectionCreator connectionFactory)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _commandProcessor = commandProcessor;
        _connectionFactory = connectionFactory;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshToken request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(RefreshToken));

        //var userClaimsPrincipal = _jwtService.GetPrincipalFromToken(request.AccessTokenData);

        //if (userClaimsPrincipal is null)
        //    throw new InvalidTokenException(userClaimsPrincipal);
        //userClaimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.NameId);

        using var conn = _connectionFactory.GetOrCreateConnection();

        var userId = await conn.QueryFirstOrDefaultAsync<long>("SELECT distinct [user_id] FROM [users].[refresh_tokens] where token = @token;", new { token = request.Token.AccessToken });

        var identityUser = await _userManager.FindByIdAsync(userId.ToString());

        if (identityUser == null)
            throw new UserNotFoundException(userId);

        var refreshToken =
            (await _commandProcessor.SendAsync(
                new GenerateRefreshToken.GenerateRefreshToken
                {
                    UserId = identityUser.Id, Token = request.Token.RefreshToken
                },
                cancellationToken)).RefreshToken;

        var accessToken =
            await _commandProcessor.SendAsync(
                new GenerateJwtToken.GenerateJwtToken(identityUser, refreshToken.Token), cancellationToken);

        return new RefreshTokenResponse(identityUser, accessToken, refreshToken.Token);
    }
}
