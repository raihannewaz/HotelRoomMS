using Common.Core.CommonModelProperties;
using Common.Core.DateTimeConversions;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Domain
{
    public class Customer : CommonModelProperty
    {
        public long Id { get; private set; }
        public string? FullName { get; private set; }
        public string? Phone { get; private set; }
        public string? Email { get; private set; }
        public string? NidNumber { get; private set; }
        public string? PassportNumber { get; private set; }
        public string? Address { get; private set; }

        public static Customer Create(string fullName, string? phone, string? email, string? nidNumber, string? passportNumber, string? address, long userId)
        {
            return new Customer
            {
                FullName = fullName,
                Phone = phone,
                Email = email,
                NidNumber = nidNumber,
                PassportNumber = passportNumber,
                Address = address,

                Created = DateTimeConversion.UTCToBST(),
                CreatedBy = userId
            };
        }

        public void Update(string fullName, string? phone, string? email, string? nidNumber, string? passportNumber, string? address, long userId)
        {
            FullName = fullName;
            Phone = phone;
            Email = email;
            NidNumber = nidNumber;
            PassportNumber = passportNumber;
            Address = address;

            LastModified = DateTimeConversion.UTCToBST();
            ModifiedBy = userId;
        }
    }
}
