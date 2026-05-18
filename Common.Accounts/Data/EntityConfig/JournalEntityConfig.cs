using Common.Accounts.Data;
using Common.Accounts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Accounts.Data.EntityConfig
{
    internal class JournalEntityConfig : IEntityTypeConfiguration<JournalMaster>
    {
        public void Configure(EntityTypeBuilder<JournalMaster> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.VoucherNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.EntryDate).IsRequired().HasColumnType("datetime");
            builder.Property(x => x.FiscalYearId).IsRequired();
            builder.Property(x => x.ReferenceId).HasDefaultValue(0L);
            builder.Property(x => x.Note).HasMaxLength(500);
            builder.Property(x => x.JournalType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.TenantId).HasMaxLength(30).HasDefaultValue(string.Empty);

            builder.Property(x => x.Created).HasColumnType("datetime");
            builder.Property(x => x.CreatedBy).HasDefaultValue(0L);
            builder.Property(x => x.LastModified).HasColumnType("datetime");
            builder.Property(x => x.ModifiedBy).HasDefaultValue(0L);


            builder.OwnsMany<JournalDetail>("JournalsDetail", c =>
            {
                // Link child to parent
                c.WithOwner().HasForeignKey(e => e.JournalMasterId);

                // Primary key for the detail table
                c.HasKey(e => e.Id);
                c.Property(e => e.Id).ValueGeneratedOnAdd();

                c.Property(e => e.AccountId).IsRequired();
                c.Property(e => e.DebitAmount).HasPrecision(18, 4).IsRequired();
                c.Property(e => e.CreditAmount).HasPrecision(18, 4).IsRequired();
                c.Property(e => e.Note).HasMaxLength(500);
                c.Property(e => e.ViceVersaAccountId).HasDefaultValue(0L);
            });

        }
    }
}
