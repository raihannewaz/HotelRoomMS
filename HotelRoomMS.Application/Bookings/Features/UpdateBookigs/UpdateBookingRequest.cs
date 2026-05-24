using HotelRoomMS.Application.Bookings.Features.CreateBookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Bookings.Features.UpdateBookigs
{
    public record UpdateBookingRequest
    {
        public long Id { get; set; }
        public long CustomerId { get; init; }
        public long RoomId { get; init; }
        public DateTime CheckIn { get; init; }
        public DateTime? ExpectedCheckOut { get; init; }
        public decimal RoomPrice { get; init; }
        public decimal TotalAmount { get; init; }
        public decimal Discount { get; init; }
        public decimal TotalPaid { get; init; }
        public string Remarks { get; init; }

        public IEnumerable<UpdateBookingGuestRequest> GuestRequests { get; init; }
    }
}
