using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Core.DateTimeConversions;
using Common.Core.Jwt;
using Common.Identity.Identity.Dtos;
using Common.Identity.Identity.Features.RefreshingToken;
using Common.Identity.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace Common.Identity.Identity.Features.GenerateRefreshToken;

public record GenerateRefreshToken : IRequest<GenerateRefreshTokenResponse>
{
    public long UserId { get; init; }
    public string Token { get; init; }
}

public class
    GenerateRefreshTokenHandler : IRequestHandler<GenerateRefreshToken, GenerateRefreshTokenResponse>
{
    private readonly IdentityContext _context;

    public GenerateRefreshTokenHandler(IdentityContext context)
    {
        _context = context;
    }

    public async Task<GenerateRefreshTokenResponse> Handle(
        GenerateRefreshToken request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(GenerateRefreshToken));

        var refreshToken = await _context.Set<Shared.Models.RefreshToken>()
            .FirstOrDefaultAsync(
                rt => rt.UserId == request.UserId && rt.Token == request.Token,
                cancellationToken);

        if (refreshToken == null)
        {
            var token = Shared.Models.RefreshToken.GetRefreshToken();

            refreshToken = new Shared.Models.RefreshToken
            {
                UserId = request.UserId,
                Token = token,
                CreatedAt = DateTimeConversion.UTCToBST(),
                ExpiredAt = DateTimeConversion.UTCToBST().AddDays(1),
                CreatedByIp = IpUtilities.GetIpAddress()
            };

            await _context.Set<Shared.Models.RefreshToken>().AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            if (!refreshToken.IsRefreshTokenValid())
                throw new InvalidRefreshTokenException(refreshToken);

            var token = Shared.Models.RefreshToken.GetRefreshToken();

            refreshToken.Token = token;
            refreshToken.ExpiredAt = DateTimeConversion.UTCToBST().AddDays(1);
            refreshToken.CreatedAt = DateTimeConversion.UTCToBST();
            refreshToken.CreatedByIp = IpUtilities.GetIpAddress();

            _context.Set<Shared.Models.RefreshToken>().Update(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        await RemoveOldRefreshTokens(request.UserId);

        return new GenerateRefreshTokenResponse(new RefreshTokenDto
        {
            Token = refreshToken.Token,
            CreatedAt = refreshToken.CreatedAt,
            ExpireAt = refreshToken.ExpiredAt,
            UserId = refreshToken.UserId,
            CreatedByIp = refreshToken.CreatedByIp,
            IsActive = refreshToken.IsActive,
            IsExpired = refreshToken.IsExpired,
            IsRevoked = refreshToken.IsRevoked,
            RevokedAt = refreshToken.RevokedAt
        });
    }


    private Task RemoveOldRefreshTokens(long userId, long? ttlRefreshToken = null)
    {
        var refreshTokens = _context.Set<Shared.Models.RefreshToken>().Where(rt => rt.UserId == userId);

        refreshTokens.ToList().RemoveAll(x => !x.IsRefreshTokenValid(ttlRefreshToken));

        return _context.SaveChangesAsync();
    }
}
