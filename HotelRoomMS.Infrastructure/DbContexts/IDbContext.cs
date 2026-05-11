using HotelRoomMS.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomMS.Infrastructure.DbContexts
{
    public interface IDbContext
    {

        DbSet<Hotel> Hotels { get; }





        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
