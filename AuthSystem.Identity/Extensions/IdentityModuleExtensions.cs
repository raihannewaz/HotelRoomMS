using System.Reflection;
using System.Text;
using AuthSystem.Identity.Data;
using AuthSystem.Identity.Models;
using AuthSystem.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AuthSystem.Identity.Extensions;

public static class IdentityModuleExtensions
{
    /// <summary>
    /// ─────────────────────────────────────────────────────────────────────────
    /// Call this ONE LINE in your API project's Program.cs:
    ///
    ///   builder.Services.AddIdentityModule(builder.Configuration);
    ///
    /// It registers:
    ///   • EF Core (IdentityAppDbContext) — uses "IdentityModule:ConnectionString"
    ///   • ASP.NET Core Identity (ApplicationUser + IdentityRole)
    ///   • JWT Authentication — uses "IdentityModule:Jwt" section
    ///   • Permission-based Authorization ([HasPermission] attribute)
    ///   • All AuthSystem controllers (auto-discovered from this assembly)
    ///   • Swagger JWT support (if Swagger is already registered)
    /// ─────────────────────────────────────────────────────────────────────────
    /// </summary>
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── 1. DbContext ───────────────────────────────────────────────────────
        var connStr = configuration["SqlServerOptions:ConnectionString"]
            ?? configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "No connection string found. Add 'SqlServerOptions:ConnectionString' to appsettings.json");

        services.AddDbContext<IdentityAppDbContext>(opt =>
            opt.UseSqlServer(connStr));

        // ── 2. Identity ────────────────────────────────────────────────────────
        services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
        {
            opt.Password.RequireDigit           = false;
            opt.Password.RequiredLength         = 6;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireUppercase       = false;
            opt.User.RequireUniqueEmail         = true;
        })
        .AddEntityFrameworkStores<IdentityAppDbContext>()
        .AddDefaultTokenProviders();

        // ── 3. JWT Authentication ──────────────────────────────────────────────
        var jwtSection = configuration.GetSection("IdentityModule:Jwt");
        var key        = Encoding.UTF8.GetBytes(
            jwtSection["Key"] ?? throw new InvalidOperationException(
                "JWT Key is missing. Add 'IdentityModule:Jwt:Key' to appsettings.json"));

        services
            .AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = jwtSection["Issuer"],
                    ValidAudience            = jwtSection["Audience"],
                    IssuerSigningKey         = new SymmetricSecurityKey(key),
                    ClockSkew                = TimeSpan.Zero
                };
            });

        // ── 4. Permission-based Authorization ─────────────────────────────────
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddAuthorization();

        // ── 5. Internal services ───────────────────────────────────────────────
        services.AddScoped<ITokenService, TokenService>();

        // ── 6. Auto-discover controllers from AuthSystem.Identity assembly ─────
        //    This is the key step — your API's AddControllers() will pick these up.
        services.AddControllers()
            .PartManager.ApplicationParts.Add(
                new AssemblyPart(typeof(IdentityModuleExtensions).Assembly));

        return services;
    }

    /// <summary>
    /// Adds JWT Bearer security definition to Swagger.
    /// Call after AddSwaggerGen() in your Program.cs:
    ///
    ///   builder.Services.AddSwaggerGen();
    ///   builder.Services.AddIdentityModuleSwagger();
    /// </summary>
    public static IServiceCollection AddIdentityModuleSwagger(
        this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name         = "Authorization",
                Type         = SecuritySchemeType.Http,
                Scheme       = "Bearer",
                BearerFormat = "JWT",
                In           = ParameterLocation.Header,
                Description  = "Paste your JWT token here (without 'Bearer ' prefix)"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    []
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Seeds default roles, permissions, and the SuperAdmin account.
    /// Call AFTER app.Build():
    ///
    ///   await app.SeedIdentityModuleAsync();
    /// </summary>
    public static async Task SeedIdentityModuleAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        // Auto-migrate on startup
        var db = scope.ServiceProvider.GetRequiredService<IdentityAppDbContext>();
        await db.Database.MigrateAsync();  // ← applies any pending migrations

        await IdentityModuleSeeder.SeedAsync(scope.ServiceProvider);
    }
}
