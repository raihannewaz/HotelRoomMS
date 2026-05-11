using BuildingBlocks.Core.Exception.Types;
using BuildingBlocks.Core.Extensions;
using Common.Identity.Shared.Models;

namespace Common.Identity.Identity.Users.Features.UpdatingUserState;

internal class UserStateCannotBeChangedException : AppException
{
    public UserState State { get; }
    public long UserId { get; }

    public UserStateCannotBeChangedException(UserState state, long userId)
        : base($"User state cannot be changed to: '{state.ToName()}' for user with ID: '{userId}'.")
    {
        State = state;
        UserId = userId;
    }
}
