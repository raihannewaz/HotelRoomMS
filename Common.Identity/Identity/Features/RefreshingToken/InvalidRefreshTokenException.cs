using Common.Core.Exceptions;

namespace Common.Identity.Identity.Features.RefreshingToken;

public class InvalidRefreshTokenException : BadRequestException
{
    public InvalidRefreshTokenException(Shared.Models.RefreshToken? refreshToken) : base(
        $"refresh token {refreshToken?.Token} is invalid!")
    {
    }
}
