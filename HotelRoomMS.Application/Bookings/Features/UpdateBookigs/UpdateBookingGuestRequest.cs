using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Bookings.Features.UpdateBookigs
{
    public record UpdateBookingGuestRequest
    {
        public long Id { get; init; }
        public string? GuestName { get; init; }
        public string? Relation { get; init; }
        public string? Phone { get; init; }
        public int Age { get; init; }
        public bool IsPrimary { get; init; }
    }
}
