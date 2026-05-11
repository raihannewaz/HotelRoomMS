using Common.Identity.Identity;

namespace Common.Identity.Users.Features.UpdateUser.Request;

public record UpdateUserRequest(
    long Id,
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword,
    string ActionType)
{
    public IEnumerable<string> Roles { get; init; } = new List<string> { Constants.Role.User };
}
