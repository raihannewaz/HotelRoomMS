using Common.Accounts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Accounts.Data.EntityConfig
{
    internal class GroupEntityConfig : IEntityTypeConfiguration<COAGroup>
    {
        public void Configure(EntityTypeBuilder<COAGroup> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.ParentId).HasDefaultValue(0L);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.IsActive).HasDefaultValue(true);


            builder.Property(x => x.Created).HasColumnType("datetime");
            builder.Property(x => x.CreatedBy).HasDefaultValue(0L);
            builder.Property(x => x.LastModified).HasColumnType("datetime");
            builder.Property(x => x.ModifiedBy).HasDefaultValue(0L);
        }
    }
}
