using System.Collections.Immutable;
using System.Security.Claims;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.Jwt;
using Common.Identity.Shared.Data;
using Common.Identity.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Common.Identity.Identity.Features.GenerateJwtToken;

public record GenerateJwtToken(ApplicationUser User, string RefreshToken) : IRequest<string>;

public class GenerateRefreshTokenCommandHandler : IRequestHandler<GenerateJwtToken, string>
{
    private readonly ILogger<GenerateRefreshTokenCommandHandler> _logger;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IdentityContext _context;

    public GenerateRefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        IdentityContext context,
        ILogger<GenerateRefreshTokenCommandHandler> logger,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _context = context;
        _logger = logger;
        _roleManager = roleManager;
        this.configuration = configuration;
    }

    public async Task<string> Handle(
        GenerateJwtToken request,
        CancellationToken cancellationToken)
    {
        var identityUser = request.User;

        var apiKey = configuration["JwtOptions:SecretKey"] ?? string.Empty;

        var allClaims = await GetClaimsAsync(request.User.UserName);
        var fullName = $"{identityUser.FirstName} {identityUser.LastName}";

        string permissions = string.Empty;

        if (allClaims.Roles.Count > 0)
        {
            foreach (var role in allClaims.Roles)
            {
                var roleObject = await _roleManager.FindByNameAsync(role);
                var permission = await _roleManager.GetClaimsAsync(roleObject);
                permissions += ", " + string.Join(", ", permission.Select(s => s.Type));
            }
        }

        List<string> listOfPermissions = [];

        if (permissions.Length > 0)
            listOfPermissions = new(permissions[1..].Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).OrderBy(s => s));

        var accessToken = _jwtService.GenerateJwtToken(
            identityUser.UserName,
            identityUser.Email,
            identityUser.Id.ToString(),
            apiKey,
            identityUser.EmailConfirmed || identityUser.PhoneNumberConfirmed,
            fullName,
            request.RefreshToken,
            allClaims.UserClaims.ToImmutableList(),
            allClaims.Roles.ToImmutableList(),
            listOfPermissions.Distinct().ToImmutableList());

        _logger.LogInformation("access-token generated, \n: {AccessToken}", accessToken);

        return accessToken;
    }

    public async Task<(IList<Claim> UserClaims, IList<string> Roles, IList<string> PermissionClaims)>
        GetClaimsAsync(string userName)
    {
        var appUser = await _userManager.FindByNameAsync(userName);
        var userClaims =
            (await _userManager.GetClaimsAsync(appUser)).Where(x => x.Type != CustomClaimTypes.Permission).ToList();
        var roles = await _userManager.GetRolesAsync(appUser);

        var permissions = (await _userManager.GetClaimsAsync(appUser))
            .Where(x => x.Type == CustomClaimTypes.Permission)?.Select(x => x
                .Value).ToList();

        return (UserClaims: userClaims, Roles: roles, PermissionClaims: permissions);
    }
}
