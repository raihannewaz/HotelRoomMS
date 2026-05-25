using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Bookings.Features.CreateBookings;

public record CreateBookingRequest
{
    public string BookingNumber { get; init; }
    public long CustomerId { get; init; }
    public long RoomId { get; init; }
    public DateTime CheckIn { get; init; }
    public DateTime? ExpectedCheckOut { get; init; }
    public decimal RoomPrice { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal Discount { get; init; }
    public decimal TotalPaid { get; init; }
    public string Remarks { get; init; }
    public string PaymentMethode { get; init; }

    public IEnumerable<CreateBookingGuestRequest> GuestRequests { get; init; }
}
