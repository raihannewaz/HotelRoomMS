using Common.Abstractions.Core;
using Common.Abstractions.Core.Extensions;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Abstractions.Messaging;
using Common.Abstractions.Persistence;
using Common.Core.Command;
using Common.Core.Email;
using Common.Core.Extensions;
using Common.Core.Jwt;
using Common.Core.Messaging;
using Common.Identity.Identity;
using Common.Identity.Identity.Data;
using Common.Identity.Roles;
using Common.Identity.Shared.Data;
using Common.Identity.Shared.Extensions.ApplicationBuilderExtensions;
using Common.Identity.Shared.Extensions.ServiceCollectionExtensions;
using Common.Identity.Users;
using Common.Sql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Common.Identity;

public static class IdentityModuleConfiguration
{
    public static IServiceCollection AddAllIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {


        services.AddScoped<IDataSeeder, IdentityDataSeeder>();

        services.AddInfrastructure(configuration);
        services.AddIdentityServices(configuration, environment);
        services.AddUsersServices(configuration);
        //services.AddTenantServices();
        services.AddRolesServices(configuration);
        services.AddControllers();

        services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
        services.Configure<SqlServerOptions>(configuration.GetSection("SqlServerOptions"));


        var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
        var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey ?? throw new ArgumentNullException("SecretKey cannot be null"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "nameid",
                    RoleClaimType = "Role"
                };
            });

        services.AddScoped<IBus, NullBus>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICommandProcessor, CommandProcessor>();

        services.AddSingleton<IEmailSender, SendGridEmailSender>();

        services.AddSingleton<IEmailSender, MailKitEmailSender>();

        services.AddMyMediaTR(typeof(IdentityModuleConfiguration).Assembly);

        services.AddScoped<IDbConnectionCreator, SqlServerConnectionFactory>();

        services.AddDbContext<IdentityContext>((serviceProvider, options) =>
        {
            var sqlOptions = serviceProvider.GetRequiredService<IOptions<SqlServerOptions>>().Value;
            options.UseSqlServer(sqlOptions.ConnectionString, sql => sql.MigrationsAssembly("HotelRoomMS.Infrastructure"));
        });



        return services;
    }


    public static async Task ConfigureModule(
        IApplicationBuilder app,
        IConfiguration configuration,
        ILogger logger,
        IWebHostEnvironment environment)
    {
       
            await app.ApplicationServices.StartHostedServices();
            app.UseIdentityServer();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.SubscribeAllMessageFromAssemblyOfType<IdentityRoot>();

        await app.ApplyDatabaseMigrations(logger);
        await app.SeedData(logger, environment);
        logger.LogInformation("Current environment: {env}", environment.EnvironmentName);

    }



    //public void MapEndpoints(IEndpointRouteBuilder endpoints)
    //{

    //    endpoints.MapGet("identity", (HttpContext context) =>
    //    {
    //        var requestId = context.Request.Headers.TryGetValue("X-Request-Id", out var requestIdHeader)
    //            ? requestIdHeader.FirstOrDefault()
    //            : string.Empty;

    //        return $"Identity Service Apis, RequestId: {requestId}";
    //    }).ExcludeFromDescription();

    //}
}
