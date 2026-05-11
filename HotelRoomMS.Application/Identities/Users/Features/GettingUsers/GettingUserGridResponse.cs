using Common.Core.Query;
using Common.CustomIdentity.Dto;

namespace BlogAppManage.Application.Identities.Users.Features.GettingUsers
{
    public record GettingUserGridResponse(ListResultModel<UserDto> User);

    public record GettingUserResponse(List<UserDto> User);
}
