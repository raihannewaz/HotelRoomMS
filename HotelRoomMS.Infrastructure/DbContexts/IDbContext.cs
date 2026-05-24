using HotelRoomMS.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomMS.Infrastructure.DbContexts
{
    public interface IDbContext
    {

        DbSet<Hotel> Hotels { get; }
        DbSet<Customer> Customers { get; }
        DbSet<RoomType> RoomTypes { get; }
        DbSet<Room> Rooms { get; }
        DbSet<Booking> Bookings { get; }





        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
