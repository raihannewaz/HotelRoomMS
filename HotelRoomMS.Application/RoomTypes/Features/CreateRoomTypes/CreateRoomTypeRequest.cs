using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.RoomTypes.Features.CreateRoomTypes
{
    public record CreateRoomTypeRequest()
    {
        public string Name { get; init; }
        public decimal BasePrice { get; init; }
    }
}
