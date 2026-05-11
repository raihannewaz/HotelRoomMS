using BlogAppManage.Application.Identities.Users.Features.GettingUsers;
using Common.CustomIdentity.Dto;
using Common.CustomIdentity.Models;

namespace BlogAppManage.Application.Identities.Users
{
    public static class UserMapper
    {
        public static UserDto QueryResponse(User entity)
        {
            return new UserDto
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                UserName = entity.UserName,
                PhoneNumber = entity.PhoneNumber,
                IsActive = entity.IsActive,
                EmailConfirmed = entity.EmailConfirmed,
                PhoneNumberConfirmed = entity.PhoneNumberConfirmed
            };
        }

        //public static Category CreateWithDto(CategoryDto dto)
        //{
        //    return Category.Create(dto.ParentId, dto.CategoryName);
        //}

        //public static void UpdateWithDto(Category category, CategoryDto dto)
        //{
        //    category.Update(dto.ParentId, dto.CategoryName);
        //}

        public static GettingUserGrid GetRequestMap(GettingUserGridRequest request)
        {
            return new GettingUserGrid()
            {
                Includes = request.Includes,
                Filters = request.Filters,
                Sorts = request.Sorts,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

    }
}
