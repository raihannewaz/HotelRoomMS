using HotelRoomMS.Application.Hotels.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Hotels.Features.GetHotelsById
{
    public record GetHotelByIdResponse(HotelDto Hotels);
}
