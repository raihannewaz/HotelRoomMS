using Common.Abstractions.Persistence;
using Common.Identity.Identity.Data;
using Common.Identity.Shared.Extensions.ServiceCollectionExtensions;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Identity;

internal static class IdentityConfigs
{

    internal static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddCustomIdentity(
            configuration,
            $"{nameof(IdentityOptions)}");

        services.AddScoped<IDataSeeder, IdentityDataSeeder>();


            services.AddCustomIdentityServer();


        return services;
    }
}
