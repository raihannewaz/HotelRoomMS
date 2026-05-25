using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.RoomTypes.Dto
{
    public record RoomTypeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; }
    }
}
