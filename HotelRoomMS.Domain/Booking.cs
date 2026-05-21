using Common.Core.CommonModelProperties;
using Common.Core.DateTimeConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Domain
{
    public class Booking : CommonModelProperty
    {
        public long Id { get; private set; }
        public string BookingNumber { get; private set; }
        public long CustomerId { get; private set; }
        public long RoomId { get; private set; }
        public DateTime CheckIn { get; private set; }
        public DateTime? ExpectedCheckOut { get; private set; }
        public DateTime? CheckOut { get; private set; } 
        public decimal RoomPrice { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal Discount { get; private set; }
        public decimal TotalPaid { get; private set; }
        public string Remarks { get; private set; }
        public bool IsCancelled { get; private set; }
        public bool IsActive { get; private set; }

        public static Booking Create(long id, string bookingNumber, long customerId, long roomId, DateTime checkIn, DateTime? expectedCheckOut, decimal roomPrice, string remarks, decimal totalPaid, long userId)
        {
            return new Booking
            {
                Id = id,
                BookingNumber = bookingNumber,
                CustomerId = customerId,
                RoomId = roomId,
                CheckIn = checkIn,
                ExpectedCheckOut = expectedCheckOut,
                RoomPrice = roomPrice,
                TotalPaid = totalPaid,
                Remarks = remarks,
                IsCancelled = false,
                IsActive = true,
   

                Created = DateTimeConversion.UTCToBST(),
                CreatedBy = userId
            };
        }

        public void CheckOutRoom(DateTime checkOut, decimal totalAmount, decimal discount, long userId)
        {
            CheckOut = checkOut;
            TotalAmount = totalAmount;
            Discount = discount;

            LastModified = DateTimeConversion.UTCToBST();
            ModifiedBy = userId;
        }

        public void CancelBooking(decimal totalAmount, decimal discount, long userId)
        {

        }
    }
}
