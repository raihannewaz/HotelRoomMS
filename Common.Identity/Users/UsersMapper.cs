using Common.Core.DateTimeConversions;
using Common.Identity.Shared.Models;
using Common.Identity.Users.Dtos;
using Common.Identity.Users.Features.GettingUsers;

namespace Common.Identity.Users;

public static class UsersMapper
{

    public static IdentityUserDto QueryResponse(ApplicationUser entity)
    {
        return new IdentityUserDto
        {
            Id = entity.Id,
            UserName = entity.UserName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber ?? "",
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            LastLoggedInAt = entity.LastLoggedInAt ?? DateTimeConversion.UTCToBST().Date,
            RefreshTokens = entity.RefreshTokens?.Select(r => r.Token).ToList() ?? new List<string>(),

        };
    }




    public static GetUsersGrid GetRequestMap(GetUsersRequest request)
    {
        return new GetUsersGrid()
        {
            Includes = request.Includes,
            Filters = request.Filters,
            Sorts = request.Sorts,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
