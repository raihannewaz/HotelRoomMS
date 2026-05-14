using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Customers.Features.CreateCustomers
{
    public record CreateCustomerRequest
    {
        public string FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? NidNumber { get; set; }
        public string? PassportNumber { get; set; }
        public string? Address { get; set; }
    }
}
