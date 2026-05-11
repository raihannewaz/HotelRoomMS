using Common.Identity.Shared.Data;
using Common.Identity.Shared.Models;
using Microsoft.AspNetCore.Identity;
namespace Common.Identity.Shared.Extensions.ServiceCollectionExtensions;

public static partial class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddCustomIdentity(
        this WebApplicationBuilder builder,
        IConfiguration configuration,
        string optionSection = nameof(IdentityOptions),
        Action<IdentityOptions>? configure = null)
    {
        AddCustomIdentity(builder.Services, configuration, optionSection, configure);

        return builder;
    }

    public static IServiceCollection AddCustomIdentity(
        this IServiceCollection services,
        IConfiguration configuration,
        string optionSection = nameof(IdentityOptions),
        Action<IdentityOptions>? configure = null)
    {

        services.AddIdentity<ApplicationUser, ApplicationRole>(
                options =>
                {
                    // Password settings.
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequiredUniqueChars = 1;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    options.User.RequireUniqueEmail = true;

                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;

                    if (configure is { })
                        configure.Invoke(options);
                })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

        if (configuration.GetSection(optionSection) is not null)
            services.Configure<IdentityOptions>(configuration.GetSection(optionSection));

        return services;
    }
}
