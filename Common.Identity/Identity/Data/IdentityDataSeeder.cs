using Common.Abstractions.Persistence;
using Common.Identity.Roles.Data;
using Common.Identity.Shared.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Common.Identity.Identity.Data;

public class IdentityDataSeeder : IDataSeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityDataSeeder(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAllAsync()
    {
        await SeedRoles();
        await SeedUsers();
        await SeedClaims();
    }

    private async Task SeedRoles()
    {
        if (!await _roleManager.RoleExistsAsync(ApplicationRole.Admin.Name))
            await _roleManager.CreateAsync(ApplicationRole.Admin);

        if (!await _roleManager.RoleExistsAsync(ApplicationRole.User.Name))
            await _roleManager.CreateAsync(ApplicationRole.User);

        if (!await _roleManager.RoleExistsAsync(ApplicationRole.Guest.Name))
            await _roleManager.CreateAsync(ApplicationRole.Guest);

        if (!await _roleManager.RoleExistsAsync(ApplicationRole.Manager.Name))
            await _roleManager.CreateAsync(ApplicationRole.Manager);
    }

    private async Task SeedUsers()
    {
        if (await _userManager.FindByEmailAsync("raihan@admin.com") == null)
        {
            var user = new ApplicationUser
            {
                //Id = 1,
                UserName = "superadmin",
                FirstName = "Raihan",
                LastName = "Newaz",
                Email = "raihan@admin.com"
            };

            var result = await _userManager.CreateAsync(user, "123456");

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, ApplicationRole.Admin.Name);
        }

        if (await _userManager.FindByEmailAsync("raihan2@admin.com") == null)
        {
            var user = new ApplicationUser
            {
                //Id = 2,
                UserName = "admin",
                FirstName = "admin",
                LastName = "Newaz",
                Email = "raihan2@admin.com"
            };


            var result = await _userManager.CreateAsync(user, "123456");

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user {user.Email}: {errors}");
            }

            await _userManager.AddToRoleAsync(user, ApplicationRole.Admin.Name);

        }
    }

    private async Task SeedClaims()
    {
        var role = await _roleManager.FindByNameAsync("admin");
        if (string.Equals(role?.NormalizedName, "ADMIN", StringComparison.Ordinal))
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            if (claims.Count == 0)
            {
                foreach (var item in AdminPermission.ListOfPermissions)
                {
                    var claim = new Claim(item.Value, item.Value.ToUpper());
                    await _roleManager.AddClaimAsync(role, claim);
                }
            }
        }
    }
}
