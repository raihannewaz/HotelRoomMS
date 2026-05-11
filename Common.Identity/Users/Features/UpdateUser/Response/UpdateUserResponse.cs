using Common.Identity.Users.Dtos;

namespace Common.Identity.Users.Features.UpdateUser.Response;

internal record UpdateUserResponse(IdentityUserDto? UserIdentity);
