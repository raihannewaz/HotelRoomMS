using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthSystem.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthSystem.Identity.Services;

public interface ITokenService
{
    Task<(string token, DateTime expiresAt)> GenerateTokenAsync(ApplicationUser user);
}

public class TokenService(
    IConfiguration config,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : ITokenService
{
    public async Task<(string token, DateTime expiresAt)> GenerateTokenAsync(ApplicationUser user)
    {
        var jwtSection = config.GetSection("IdentityModule:Jwt");
        var key        = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var creds      = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt  = DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["ExpiryMinutes"] ?? "60"));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new("fullName",                    user.FullName),
        };

        // Add role claims
        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // Add permission claims from all roles + direct user claims
        var permissionSet = new HashSet<string>();

        foreach (var role in roles)
        {
            var identityRole = await roleManager.FindByNameAsync(role);
            if (identityRole is null) continue;
            var roleClaims = await roleManager.GetClaimsAsync(identityRole);
            foreach (var c in roleClaims.Where(c => c.Type == "Permission"))
                permissionSet.Add(c.Value);
        }

        var userClaims = await userManager.GetClaimsAsync(user);
        foreach (var c in userClaims.Where(c => c.Type == "Permission"))
            permissionSet.Add(c.Value);

        claims.AddRange(permissionSet.Select(p => new Claim("Permission", p)));

        var token = new JwtSecurityToken(
            issuer:             jwtSection["Issuer"],
            audience:           jwtSection["Audience"],
            claims:             claims,
            expires:            expiresAt,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
