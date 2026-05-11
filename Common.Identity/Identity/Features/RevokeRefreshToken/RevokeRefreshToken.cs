using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Core.DateTimeConversions;
using Common.Identity.Identity.Exceptions;
using Common.Identity.Identity.Features.RefreshingToken;
using Common.Identity.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace Common.Identity.Identity.Features.RevokeRefreshToken;

public record RevokeRefreshToken(string RefreshToken) : IRequest;

internal class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshToken>
{
    private readonly IdentityContext _context;

    public RevokeRefreshTokenHandler(IdentityContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(
        RevokeRefreshToken request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(RevokeRefreshToken));

        var refreshToken = await _context.Set<global::Common.Identity.Shared.Models.RefreshToken>()
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken: cancellationToken);

        if (refreshToken == null)
            throw new RefreshTokenNotFoundException(refreshToken);

        if (!refreshToken.IsRefreshTokenValid())
            throw new InvalidRefreshTokenException(refreshToken);

        refreshToken.RevokedAt = DateTimeConversion.UTCToBST();
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
