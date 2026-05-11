using BuildingBlocks.Core.Messaging;
using Common.Identity.Shared.Models;

namespace Common.Identity.Identity.Users.Features.UpdatingUserState.Events.Integration;

public record UserStateUpdated(long UserId, UserState OldUserState, UserState NewUserState) : IntegrationEvent;
