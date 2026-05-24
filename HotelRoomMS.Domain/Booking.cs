using Common.Core.CommonModelProperties;
using Common.Core.DateTimeConversions;
using Common.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        private readonly List<BookingGuest> _bookingGuests = new();
        public IReadOnlyCollection<BookingGuest> BookingGuests => _bookingGuests.AsReadOnly();

        public static Booking Create(long id, string bookingNumber, long customerId, long roomId, DateTime checkIn, DateTime? expectedCheckOut, decimal roomPrice, string remarks, decimal totalAmount, decimal totalPaid, long userId)
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
                TotalAmount = totalAmount,
                TotalPaid = totalPaid,
                Remarks = remarks,
                IsCancelled = false,
                IsActive = true,
   

                Created = DateTimeConversion.UTCToBST(),
                CreatedBy = userId
            };
        }

        public void Update(long customerId, long roomId, DateTime checkIn, DateTime? expectedCheckOut, decimal roomPrice, string remarks, decimal totalAmount, decimal totalPaid, long userId)
        {
            CustomerId = customerId;
            RoomId = roomId;
            CheckIn = checkIn;
            ExpectedCheckOut = expectedCheckOut; 
            RoomPrice = roomPrice;
            TotalAmount = totalAmount;
            Remarks = remarks;
            TotalPaid = totalPaid;

            ModifiedBy = userId;
            LastModified = DateTimeConversion.UTCToBST();
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
            IsCancelled = true;
            TotalAmount = totalAmount;
            Discount = discount;
            ModifiedBy = userId;
            LastModified = DateTimeConversion.UTCToBST();
        }



        public void AddOrUpdateDetails(IList<BookingGuest> details)
        {
            if (details is null)
                throw new ApiException("details can not be empty.", HttpStatusCode.NoContent);

            foreach (var value in details)
            {
                var existingDetails = _bookingGuests.SingleOrDefault(x => x.Id > 0 && x.Id == value.Id);
                if (existingDetails == null)
                {
                    var newDetails = BookingGuest.Create(0, Id, value.GuestName ?? "", value.Relation ?? "", value.Phone ?? "", value.Age, value.IsPrimary);
                    _bookingGuests.Add(newDetails);
                }
                else
                {
                    existingDetails.Update(value.GuestName ?? "", value.Relation ?? "", value.Phone ?? "", value.Age, value.IsPrimary);
                }
            }

            foreach (var existingDetail in _bookingGuests.Where(s => s.Id > 0).ToList())
            {
                var detail = details.SingleOrDefault(x => x.Id > 0 && x.Id == existingDetail.Id);
                if (detail == null)
                {
                    _bookingGuests.Remove(existingDetail);
                }
            }
        }
    }
}
