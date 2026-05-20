using HotelRoomMS.Application.RoomTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.RoomTypes.Features.GetRoomTypesById;

public record GetRoomTypeByIdResponse(RoomTypeDto RoomTypes);

