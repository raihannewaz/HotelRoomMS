using Common.Core.Messaging;

namespace Common.Identity.Users.Features.RegisteringUser.Events.Integration;

public record UserRegistered(
    long IdentityId,
    string? Email,
    string UserName,
    string FirstName,
    string LastName,
    List<string>? Roles) : IntegrationEvent;
