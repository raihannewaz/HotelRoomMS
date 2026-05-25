using AuthSystem.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AuthSystem.Identity.Data;

/// <summary>
/// Called once at startup from Program.cs via:
///   await IdentityModuleSeeder.SeedAsync(app.Services);
/// </summary>
public static class IdentityModuleSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // ── 1. Create default roles ────────────────────────────────────────────
        string[] defaultRoles = ["SuperAdmin", "Admin", "Manager", "User"];
        foreach (var roleName in defaultRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // ── 2. SuperAdmin gets ALL permissions ─────────────────────────────────
        var superAdminRole = await roleManager.FindByNameAsync("SuperAdmin");
        if (superAdminRole != null)
        {
            var existing = await roleManager.GetClaimsAsync(superAdminRole);
            foreach (var permission in Permissions.GetAll())
            {
                if (!existing.Any(c => c.Type == "Permission" && c.Value == permission))
                    await roleManager.AddClaimAsync(
                        superAdminRole,
                        new System.Security.Claims.Claim("Permission", permission));
            }
        }

        // ── 3. Manager gets limited permissions ────────────────────────────────
        var managerRole = await roleManager.FindByNameAsync("Manager");
        if (managerRole != null)
        {
            var existing = await roleManager.GetClaimsAsync(managerRole);
            string[] managerPerms =
            [
                Permissions.UsersView
                // Add more as needed
            ];
            foreach (var perm in managerPerms)
            {
                if (!existing.Any(c => c.Type == "Permission" && c.Value == perm))
                    await roleManager.AddClaimAsync(
                        managerRole,
                        new System.Security.Claims.Claim("Permission", perm));
            }
        }

        // ── 4. Seed a default SuperAdmin account ───────────────────────────────
        const string adminEmail = "raihan@app.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                FullName       = "Super Admin",
                UserName       = "SuperAdmin",
                Email          = adminEmail,
                EmailConfirmed = true,
                IsActive       = true
            };

            var result = await userManager.CreateAsync(admin, "SuperRaihan");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "SuperAdmin");
        }
    }
}
