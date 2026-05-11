using Common.Core.DateTimeConversions;
using System.Security.Cryptography;

namespace Common.Identity.Shared.Models;

public class RefreshToken
{
    public long UserId { get; set; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiredAt { get; set; }
    public string CreatedByIp { get; set; }
    public bool IsExpired => DateTimeConversion.UTCToBST() >= ExpiredAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;
    public DateTime? RevokedAt { get; set; }
    public ApplicationUser ApplicationUser { get; set; }


    public static string GetRefreshToken()
    {
        var randomNumber = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);

        var refreshToken = Convert.ToBase64String(randomNumber);

        return refreshToken;
    }

    public bool IsRefreshTokenValid(double? ttlRefreshToken = null)
    {
        if (!IsActive) return false;

        if (ttlRefreshToken is not null &&
            CreatedAt.AddDays((long)ttlRefreshToken) <= DateTimeConversion.UTCToBST()) return false;

        return true;
    }
}
