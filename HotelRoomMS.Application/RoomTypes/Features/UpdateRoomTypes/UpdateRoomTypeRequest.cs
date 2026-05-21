using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.RoomTypes.Features.UpdateRoomTypes
{
    public record UpdateRoomTypeRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
    }
}
