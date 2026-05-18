using Common.Accounts.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Common.Accounts.Data;


public class AccountsDbContext(DbContextOptions<AccountsDbContext> options)
    : DbContext(options)
{
    public DbSet<COAGroup> Groups { get; set; }
    public DbSet<ChartOfAccount> ChartOfAccounts { get; set; }
    public DbSet<FiscalYear> FiscalYears { get; set; }
    public DbSet<JournalMaster> JournalsMaster { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("accounts");

        builder.Entity<COAGroup>().ToTable("Groups");
        builder.Entity<ChartOfAccount>().ToTable("ChartOfAccounts");
        builder.Entity<FiscalYear>().ToTable("FiscalYears");
        builder.Entity<JournalMaster>().ToTable("JournalsMaster");

    }
}
