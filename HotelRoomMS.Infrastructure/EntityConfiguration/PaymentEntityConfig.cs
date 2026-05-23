using HotelRoomMS.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Infrastructure.EntityConfiguration
{
    internal class PaymentEntityConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.ReferenceId).HasDefaultValue(0L);
            builder.Property(x => x.PaymentDate).HasColumnType("datetime");
            builder.Property(x => x.PaymentMethod).HasMaxLength(20);
            builder.Property(x => x.Remarks).HasMaxLength(500);
            builder.Property(x => x.Amount).HasDefaultValue(0L);


            builder.Property(x => x.Created).HasColumnType("datetime");
            builder.Property(x => x.CreatedBy).HasDefaultValue(0L);
            builder.Property(x => x.LastModified).HasColumnType("datetime");
            builder.Property(x => x.ModifiedBy).HasDefaultValue(0L);

        }
    }
}
