using Common.Identity.Users.Dtos;

namespace Common.Identity.Users.Features.GettingUerByEmail;

public record GetUserByEmailResponse(IdentityUserDto? UserIdentity);
