using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Rooms.Dto
{
    public class RoomDto
    {
        public long Id { get;  set; }
        public long HotelId { get;  set; }
        public string HotelName { get; set; }
        public long RoomTypeId { get;  set; }
        public string RoomType { get; set; }
        public string RoomNumber { get;  set; }
        public decimal PricePerDay { get;  set; }
        public bool IsBooked { get;  set; }
        public bool IsActive { get;  set; }
    }
}
