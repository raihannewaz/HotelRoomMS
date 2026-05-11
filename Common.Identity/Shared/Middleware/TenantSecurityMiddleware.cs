using Common.Identity.Shared.Services;

namespace Common.Identity.Shared.Middleware;

public class TenantSecurityMiddleware
{
    private readonly RequestDelegate next;

    public TenantSecurityMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(
        HttpContext context,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        string tenantIdentifier = context.Request.Headers["TenantId"].FirstOrDefault();
        TenantSecurityService tenantRepository = new TenantSecurityService(configuration, httpContextAccessor);

        if (string.IsNullOrEmpty(tenantIdentifier))
        {
            var apiKey = context.Request.Headers["X-Api-Key"].FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                return;
            }


            if (!await tenantRepository.GetTenantExist(apiKey))
            {
                return;
            }

            string? tenantId = await tenantRepository.GetTenantId();
            context.Request.Headers["TenantId"] = tenantId;
        }

        await next.Invoke(context);
    }
}
