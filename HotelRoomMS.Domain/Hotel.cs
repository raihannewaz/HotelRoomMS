using Common.Core.CommonModelProperties;
using Common.Core.DateTimeConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Domain
{
    public class Hotel : CommonModelProperty
    {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public string? Phone {  get; private set; }
        public string? Email { get; private set; }
        public string? Address { get; private set; }
        public bool IsActive { get; private set; }


        public static Hotel Create(long id, string name, string? phone, string? email, string? address, long createdBy)
        {
            var hotel = new Hotel();

            hotel.Id = id;
            hotel.Name = name;
            hotel.Phone = phone;
            hotel.Email = email;
            hotel.Address = address;
            hotel.Created = DateTimeConversion.UTCToBST();
            hotel.CreatedBy = createdBy;
            hotel.IsActive = true;

            return hotel;
        }


        public void Edit(string name, string? phone, string? email, string? address, long modifiedBy)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
            ModifiedBy = modifiedBy;
            LastModified = DateTimeConversion.UTCToBST();
        }

        public void ChangeStatus(bool isActive)
        {
            IsActive = isActive;
        }
    }
}
