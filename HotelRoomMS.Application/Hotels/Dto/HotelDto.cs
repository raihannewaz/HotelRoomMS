using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Hotels.Dto
{
    public record HotelDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public string? Address { get; init; }
        public bool IsActive { get; init; }
    }
}
