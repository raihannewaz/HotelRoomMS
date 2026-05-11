using HotelRoomMS.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelRoomMS.Infrastructure.EntityConfiguration
{
    internal class HotelEntityConfig : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.Id).IsUnique();

            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Phone).HasMaxLength(20);
            builder.Property(x => x.Email).HasMaxLength(50);
            builder.Property(x => x.Address).HasMaxLength(255);
            builder.Property(x => x.IsActive);


            builder.Property(x => x.Created);
            builder.Property(x => x.CreatedBy);
            builder.Property(x => x.LastModified);
            builder.Property(x => x.ModifiedBy);
        }
    }
}
