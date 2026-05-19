using AuthSystem.Identity.Data;
using Common.Accounts.Data;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Accounts.Extensions;

public static class AccountsModuleExtensions
{

    public static IServiceCollection AddAccountsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connStr = configuration["SqlServerOptions:ConnectionString"]
            ?? configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "No connection string found. Add 'SqlServerOptions:ConnectionString' to appsettings.json");

        services.AddDbContext<AccountsDbContext>(opt =>
            opt.UseSqlServer(connStr));

        services.AddControllers()
            .PartManager.ApplicationParts.Add(
                new AssemblyPart(typeof(AccountsModuleExtensions).Assembly));

        return services;
    }


        //public static async Task SeedIdentityModuleAsync(this WebApplication app)
        //{
        //    using var scope = app.Services.CreateScope();

        //    // Auto-migrate on startup
        //    var db = scope.ServiceProvider.GetRequiredService<IdentityAppDbContext>();
        //    await db.Database.MigrateAsync();  // ← applies any pending migrations

        //    await IdentityModuleSeeder.SeedAsync(scope.ServiceProvider);
        //}
    }
