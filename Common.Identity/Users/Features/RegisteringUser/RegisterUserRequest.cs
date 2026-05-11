using Common.Identity.Identity;

namespace Common.Identity.Users.Features.RegisteringUser;

public record RegisterUserRequest(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string TenantId,
    string ConfirmPassword)
{
    public IEnumerable<string> Roles { get; init; } = new List<string> { Constants.Role.User };
}
