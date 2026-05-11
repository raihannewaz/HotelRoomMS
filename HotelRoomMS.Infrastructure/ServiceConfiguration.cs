using HotelRoomMS.Infrastructure.DbContexts;
using Common.Abstractions.EFCoreConnection;
using Common.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HotelRoomMS.Infrastructure
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SqlServerOptions>(configuration.GetSection("SqlServerOptions"));

            services.AddScoped<IDbContext, AppDbContext>();
            services.AddScoped<IDbConnectionCreator, SqlServerConnectionFactory>();

            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var sqlOptions = serviceProvider.GetRequiredService<IOptions<SqlServerOptions>>().Value;
                options.UseSqlServer(sqlOptions.ConnectionString);
            });



            return services;
        }
    }
}
