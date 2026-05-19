using AuthSystem.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Identity.Data;

/// <summary>
/// Identity DbContext. Registered by AddIdentityModule().
/// Migrations are generated from your API project:
///   dotnet ef migrations add Init --project AuthSystem.Identity --startup-project MyApp.API
/// </summary>
public class IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Prefix all Identity tables to avoid collisions with your main DB tables
        builder.Entity<ApplicationUser>().ToTable("Auth_Users");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().ToTable("Auth_Roles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable("Auth_UserRoles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ToTable("Auth_UserClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ToTable("Auth_RoleClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ToTable("Auth_UserLogins");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ToTable("Auth_UserTokens");
    }
}
