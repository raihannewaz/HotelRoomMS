using HotelRoomMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Bookings.Dto
{
    public class BookingDto
    {
        public long Id { get;  set; }
        public string BookingNumber { get;  set; }
        public long CustomerId { get;  set; }
        public long RoomId { get;  set; }
        public string RoomNumber { get;  set; }
        public DateTime CheckIn { get;  set; }
        public DateTime? ExpectedCheckOut { get;  set; }
        public DateTime? CheckOut { get;  set; }
        public decimal RoomPrice { get;  set; }
        public decimal TotalAmount { get;  set; }
        public decimal Discount { get;  set; }
        public decimal TotalPaid { get;  set; }
        public string Remarks { get;  set; }
        public bool IsCancelled { get;  set; }
        public bool IsActive { get;  set; }

        public IEnumerable<BookingGuestDto> BookingGuests { get; set; }
    }
}
