using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Customers.Dto
{
    public record CustomerDto
    {
        public long Id { get; init; }
        public string? FullName { get; init; }
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public string? NidNumber { get; init; }
        public string? PassportNumber { get; init; }
        public string? Address { get; init; }
    }
}
