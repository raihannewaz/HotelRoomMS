using HotelRoomMS.Domain;
using HotelRoomMS.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Infrastructure.EntityConfiguration
{
    internal class BookingEntityConfig : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasKey(c => c.Id);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.BookingNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.CustomerId);
            builder.Property(x => x.RoomId).IsRequired();
            builder.Property(x => x.CheckIn).HasColumnType("datetime");
            builder.Property(x => x.ExpectedCheckOut).HasColumnType("datetime");
            builder.Property(x => x.CheckOut).HasColumnType("datetime");
            builder.Property(x => x.Remarks).HasMaxLength(500);
            builder.Property(x => x.RoomPrice).HasDefaultValue(0L);
            builder.Property(x => x.TotalAmount).HasDefaultValue(0L);
            builder.Property(x => x.Discount).HasDefaultValue(0L);
            builder.Property(x => x.TotalPaid).HasDefaultValue(0L);
            builder.Property(x => x.IsCancelled).HasDefaultValue(false);
            builder.Property(x => x.IsActive).HasDefaultValue(true);

            builder.Property(x => x.Created).HasColumnType("datetime");
            builder.Property(x => x.CreatedBy).HasDefaultValue(0L);
            builder.Property(x => x.LastModified).HasColumnType("datetime");
            builder.Property(x => x.ModifiedBy).HasDefaultValue(0L);


            builder.OwnsMany<BookingGuest>("BookingGuests", c =>
            {
                // Link child to parent
                c.WithOwner().HasForeignKey(e => e.BookingId);
                c.ToTable("BookingGuests", AppDbContext.DefaultSchema);

                // Primary key for the detail table
                c.HasKey(e => e.Id);
                c.Property(e => e.Id).ValueGeneratedOnAdd();

                c.Property(e => e.BookingId).IsRequired();
                c.Property(e => e.GuestName).HasMaxLength(100);
                c.Property(e => e.Relation).HasMaxLength(30);
                c.Property(e => e.Age).HasDefaultValue(0);
                c.Property(e => e.Phone).HasMaxLength(20);
                c.Property(e => e.IsPrimary);
            });
        }
    }
}
