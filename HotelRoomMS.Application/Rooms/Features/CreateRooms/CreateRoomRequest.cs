using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Rooms.Features.CreateRooms
{
    public record CreateRoomRequest
    {
        public long HotelId { get; init; }
        public long RoomTypeId { get; init; }
        public string RoomNumber { get; init; }
        public decimal PricePerDay { get; init; }

    }
}
