using Common.Accounts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Accounts.Data.EntityConfig
{
    internal class FiscalYearEntityConfig : IEntityTypeConfiguration<FiscalYear>
    {
        public void Configure(EntityTypeBuilder<FiscalYear> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.FromDate).HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ToDate).HasColumnType("datetime").IsRequired();
            builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
            builder.Property(x => x.IsClosed);
            builder.Property(x => x.IsActive);


            builder.Property(x => x.Created).HasColumnType("datetime");
            builder.Property(x => x.CreatedBy).HasDefaultValue(0L);
            builder.Property(x => x.LastModified).HasColumnType("datetime");
            builder.Property(x => x.ModifiedBy).HasDefaultValue(0L);
        }
    }
}
