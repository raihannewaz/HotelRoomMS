using Common.Core.Query;
using Common.Identity.Identity.Users.Dtos;

namespace Common.Identity.Users.Features.GettingUsers
{
    public record GetUsersResponse(ListResultModel<UserDto> IdentityUsers);
    public record GettingUserResponse(List<UserDto> Users);
}