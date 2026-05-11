using Dapper;
using Npgsql;

namespace Common.Identity.Shared.Services;

public class TenantSecurityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly IConfiguration _configuration;

    public TenantSecurityService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> GetTenantExist(string apiKey)
    {
        string tenantId = null;
        try
        {
            await using var connection =
                new NpgsqlConnection(_configuration["MultiTenant:PostgresOptions:ConnectionString"]);

            // Create a query that retrieves all books with an author name of "John Smith"
            var sql = $"SELECT CAST(COUNT(Id) AS BIT) FROM app.Tenants WHERE ApiKey = @apiKey;";

            // Use the Query method to execute the query and return a list of objects
            var tenantExist = connection.Query<bool>(sql, new { apiKey }).FirstOrDefault();

            return tenantExist;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<string?> GetTenantId()
    {
        return await Task.FromResult(_httpContextAccessor.HttpContext.Request.Headers["TenantId"].FirstOrDefault());
    }
}
