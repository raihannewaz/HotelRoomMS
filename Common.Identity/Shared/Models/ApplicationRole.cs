using System.Globalization;
using Common.Identity.Identity;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Shared.Models;

public class ApplicationRole : IdentityRole<long>
{
    public static ApplicationRole User => new()
    {
        Id = 1,
        Name = Constants.Role.User,
        NormalizedName = nameof(User).ToUpper(CultureInfo.InvariantCulture),
    };

    public static ApplicationRole Admin => new()
    {
        Id = 2,
        Name = Constants.Role.Admin,
        NormalizedName = nameof(Admin).ToUpper(CultureInfo.InvariantCulture)
    };

    public static ApplicationRole Guest => new()
    {
        Id = 3,
        Name = Constants.Role.Guest,
        NormalizedName = nameof(Guest).ToUpper(CultureInfo.InvariantCulture)
    };

    public static ApplicationRole Manager => new()
    {
        Id = 4,
        Name = Constants.Role.Manager,
        NormalizedName = nameof(Manager).ToUpper(CultureInfo.InvariantCulture)
    };
}
