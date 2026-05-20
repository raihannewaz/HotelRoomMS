using HotelRoomMS.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelRoomMS.Infrastructure.EntityConfiguration
{
    internal class HotelEntityConfig : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.ToTable("Hotels");
            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Phone).HasMaxLength(20).IsUnicode(false);
            builder.Property(x => x.Email).HasMaxLength(50).IsUnicode(false);
            builder.Property(x => x.Address).HasMaxLength(255);
            builder.Property(x => x.IsActive).HasDefaultValue(true);


            builder.Property(x => x.Created).HasColumnType("datetime");
            builder.Property(x => x.CreatedBy).HasDefaultValue(0L);
            builder.Property(x => x.LastModified).HasColumnType("datetime");
            builder.Property(x => x.ModifiedBy).HasDefaultValue(0L);
        }
    }
}
