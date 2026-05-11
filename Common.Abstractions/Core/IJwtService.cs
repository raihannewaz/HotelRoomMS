using System.Security.Claims;

namespace Common.Abstractions.Core;

public interface IJwtService
{
    string GenerateJwtToken(
        string userName,
        string email,
        string userId,
        string apiKey,
        bool? isVerified = null,
        string? fullName = null,
        string? refreshToken = null,
        IReadOnlyList<Claim>? usersClaims = null,
        IReadOnlyList<string>? rolesClaims = null,
        IReadOnlyList<string>? permissionsClaims = null);

    ClaimsPrincipal? GetPrincipalFromToken(string token);
}
