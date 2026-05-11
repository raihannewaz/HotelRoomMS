using Common.Identity.Roles.Dtos;

namespace Common.Identity.Roles.Data;

public static class AdminPermission
{
    private static readonly List<PermissionDto> permissionDtos = new List<PermissionDto>
    {

        new() { Module = "Identity", SubModule = "Users", Text = "Users", Value = "USER.CREATE" },
        new() { Module = "Identity", SubModule = "Users", Text = "Users", Value = "USER.EDIT" },
        new() { Module = "Identity", SubModule = "Users", Text = "Users", Value = "USER.VIEW" },
        new() { Module = "Identity", SubModule = "Users", Text = "Users", Value = "USER.DELETE" },

        new() { Module = "Identity", SubModule = "Roles", Text = "Roles" ,Value = "ROLE.CREATE" },
        new() { Module = "Identity", SubModule = "Roles", Text = "Roles" ,Value = "ROLE.EDIT" },
        new() { Module = "Identity", SubModule = "Roles", Text = "Roles" ,Value = "ROLE.VIEW" },
        new() { Module = "Identity", SubModule = "Roles", Text = "Roles" ,Value = "ROLE.DELETE" },



        new() { Module = "Blog", SubModule = "Category", Text = "Category", Value = "CATEGORY.CREATE" },
        new() { Module = "Blog", SubModule = "Category", Text = "Category", Value = "CATEGORY.EDIT" },
        new() { Module = "Blog", SubModule = "Category", Text = "Category", Value = "CATEGORY.VIEW" },
        new() { Module = "Blog", SubModule = "Category", Text = "Category", Value = "CATEGORY.DELETE" },

        new() { Module = "Blog", SubModule = "Tag", Text = "Tag", Value = "TAG.CREATE" },
        new() { Module = "Blog", SubModule = "Tag", Text = "Tag", Value = "TAG.EDIT" },
        new() { Module = "Blog", SubModule = "Tag", Text = "Tag", Value = "TAG.VIEW" },
        new() { Module = "Blog", SubModule = "Tag", Text = "Tag", Value = "TAG.DELETE" },

        new() { Module = "Blog", SubModule = "Book", Text = "Book", Value = "BOOK.CREATE" },
        new() { Module = "Blog", SubModule = "Book", Text = "Book", Value = "BOOK.EDIT" },
        new() { Module = "Blog", SubModule = "Book", Text = "Book", Value = "BOOK.VIEW" },
        new() { Module = "Blog", SubModule = "Book", Text = "Book", Value = "BOOK.DELETE" },

        new() { Module = "Blog", SubModule = "Post", Text = "Post", Value = "POST.CREATE" },
        new() { Module = "Blog", SubModule = "Post", Text = "Post", Value = "POST.EDIT" },
        new() { Module = "Blog", SubModule = "Post", Text = "Post", Value = "POST.VIEW" },
        new() { Module = "Blog", SubModule = "Post", Text = "Post", Value = "POST.DELETE" },


    };
    public static IEnumerable<PermissionDto> ListOfPermissions = permissionDtos;
}
