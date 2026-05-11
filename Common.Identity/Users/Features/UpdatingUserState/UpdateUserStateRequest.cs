using Common.Identity.Shared.Models;

namespace Common.Identity.Identity.Users.Features.UpdatingUserState;

public record UpdateUserStateRequest
{
    public UserState UserState { get; init; }
}
