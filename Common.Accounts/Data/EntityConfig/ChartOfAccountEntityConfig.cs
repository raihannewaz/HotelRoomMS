using Common.Accounts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Accounts.Data.EntityConfig
{
    internal class ChartOfAccountEntityConfig : IEntityTypeConfiguration<ChartOfAccount>
    {
        public void Configure(EntityTypeBuilder<ChartOfAccount> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Id).ValueGeneratedNever();

            //Relationship Configuration without navigation properties
            builder.HasOne<COAGroup>().WithMany().HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.NoAction);

            builder.Property(x => x.ParentId).HasDefaultValue(0L);
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.IsActive).HasDefaultValue(true);

            builder.Property(x => x.Created).HasColumnType("datetime");
            builder.Property(x => x.CreatedBy).HasDefaultValue(0L);
            builder.Property(x => x.LastModified).HasColumnType("datetime");
            builder.Property(x => x.ModifiedBy).HasDefaultValue(0L);
        }
    }
}
