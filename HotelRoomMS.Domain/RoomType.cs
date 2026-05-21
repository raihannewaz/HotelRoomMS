using Common.Core.CommonModelProperties;
using Common.Core.DateTimeConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Domain
{
    public class RoomType : CommonModelProperty
    {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public decimal BasePrice { get; private set; }
        public bool IsActive { get; private set; }


        public static RoomType Create(string name, decimal basePrice, long userId)
        {
            return new RoomType
            {
                Name = name,
                BasePrice = basePrice,
                IsActive = true,
                CreatedBy = userId,
                Created = DateTimeConversion.UTCToBST()
            };
        }


        public void Update(string name, decimal basePrice, long userId)
        {
            Name = name;
            BasePrice = basePrice;
            ModifiedBy = userId;
            LastModified = DateTimeConversion.UTCToBST();
        }

        public void ActiveStatusChange(long userId)
        {
            IsActive = !IsActive;
            ModifiedBy = userId;
            LastModified = DateTimeConversion.UTCToBST();
        }
    }
}
