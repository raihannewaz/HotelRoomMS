using Common.Abstractions.Core;
using Common.Core.Jwt;

namespace Common.Identity.Shared.Extensions.ServiceCollectionExtensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddTransient<ISecurityContextAccessor, SecurityContextAccessor>();

        return services;
    }
}
