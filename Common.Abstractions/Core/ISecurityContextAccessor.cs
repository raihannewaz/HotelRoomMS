namespace Common.Abstractions.Core;

public interface ISecurityContextAccessor
{
    string UserId { get; }
    string Role { get; }
    string ApiKey { get; }
    string TenantId { get; }
    string JwtToken { get; }
    bool IsAuthenticated { get; }
    List<string> Permissions { get; }
}
