using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Bookings.Dto
{
    public class BookingGuestDto
    {
        public long Id { get;  set; }
        public long BookingId { get;  set; }
        public string? GuestName { get;  set; }
        public string? Relation { get;  set; }
        public string? Phone { get;  set; }
        public int Age { get;  set; }
        public bool IsPrimary { get;  set; }
    }
}
