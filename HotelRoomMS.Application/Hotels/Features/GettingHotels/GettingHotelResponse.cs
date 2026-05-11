using Common.Core.Query;
using HotelRoomMS.Application.Hotels.Dto;
using HotelRoomMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Hotels.Features.GettingHotels
{
    public record GettingHotelResponse(ListResultModel<HotelDto> Hotels);
}
