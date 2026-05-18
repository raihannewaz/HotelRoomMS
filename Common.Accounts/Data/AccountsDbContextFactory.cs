using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Data
{
    internal class AccountsDbContextFactory : IDesignTimeDbContextFactory<AccountsDbContext>
    {
        public AccountsDbContext CreateDbContext(string[] args)
        {
            // Walks up from the library folder to find the API's appsettings.json
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../HotelRoomMS.Api");

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connStr = config["SqlServerOptions:ConnectionString"]
                       ?? config.GetConnectionString("Default")!;

            var optionsBuilder = new DbContextOptionsBuilder<AccountsDbContext>();
            optionsBuilder.UseSqlServer(connStr);

            return new AccountsDbContext(optionsBuilder.Options);
        }
    }
}
