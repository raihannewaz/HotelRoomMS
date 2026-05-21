using Common.Core.CommonModelProperties;
using Common.Core.DateTimeConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Domain
{
    public class Room : CommonModelProperty
    {
        public long Id { get; private set; }
        public long HotelId { get; private set; }
        public long RoomTypeId { get; private set; }
        public string RoomNumber { get; private set; }
        public decimal PricePerDay { get; private set; }
        public bool IsBooked { get; private set; }
        public bool IsActive { get; private set; }

        public static Room Create(long id, long hotelId, long roomTypeId, string roomNumber, decimal pricePerDay, long userId)
        {
            return new Room
            {
                Id = id,
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                RoomNumber = roomNumber,
                PricePerDay = pricePerDay,
                IsBooked = false,
                IsActive = true,
                Created = DateTimeConversion.UTCToBST(),
                CreatedBy = userId
            };
        }

        public void Update(long hotelId, long roomTypeId, string roomNumber, decimal pricePerDay, long userId)
        {
            HotelId = hotelId;
            RoomTypeId = roomTypeId;
            RoomNumber = roomNumber;
            PricePerDay = pricePerDay;
            LastModified = DateTimeConversion.UTCToBST();
            ModifiedBy = userId;
        }


        public void ActiveStatusChange(long userId)
        {
            IsActive = !IsActive;
            LastModified = DateTimeConversion.UTCToBST();
            ModifiedBy = userId;
        }
    }
}
