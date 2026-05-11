using Common.Identity.Users.Dtos;

namespace Common.Identity.Users.Features.RegisteringUser;

internal record RegisterUserResponse(IdentityUserDto? UserIdentity);
