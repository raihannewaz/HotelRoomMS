using System.Security.Claims;
using Common.Abstractions.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Core.Jwt;

public class SecurityContextAccessor : ISecurityContextAccessor
{
    private readonly ILogger<SecurityContextAccessor> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SecurityContextAccessor(
        IHttpContextAccessor httpContextAccessor,
        ILogger<SecurityContextAccessor> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value;
            return userId;
        }
    }


    public string JwtToken
    {
        get
        {
            return _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"];
        }
    }

    public string ApiKey
    {
        get
        {
            return _httpContextAccessor.HttpContext?.Request?.Headers["X-Api-Key"];
        }
    }

    public string TenantId
    {
        get
        {
            return _httpContextAccessor.HttpContext?.Request?.Headers["TenantId"];
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            var isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identities?.FirstOrDefault()?.IsAuthenticated;
            return isAuthenticated.HasValue && isAuthenticated.Value;
        }
    }

    public string Role
    {
        get
        {
            var role = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            return role;
        }
    }

    public List<string> Permissions
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.Claims.Where(x => x.Type == "permission").Select(x => x.Value).ToList();
        }
    }
}
