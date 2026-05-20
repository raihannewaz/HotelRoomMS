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
    internal class CustomerEntityConfig : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Phone).HasMaxLength(20).IsUnicode(false);
            builder.Property(x => x.Email).HasMaxLength(50).IsUnicode(false);
            builder.Property(x => x.NidNumber).HasMaxLength(20).IsUnicode(false);
            builder.Property(x => x.PassportNumber).HasMaxLength(20).IsUnicode(false);
            builder.Property(x => x.Address).HasMaxLength(150);


            builder.Property(x => x.Created).HasColumnType("datetime");
            builder.Property(x => x.CreatedBy).HasDefaultValue(0L);
            builder.Property(x => x.LastModified).HasColumnType("datetime");
            builder.Property(x => x.ModifiedBy).HasDefaultValue(0L);
        }
    }
}
