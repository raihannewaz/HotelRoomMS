using Common.Core.Query;
using HotelRoomMS.Application.RoomTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.RoomTypes.Features.GettingRoomTypes;

public record GettingRoomTypeResponse(ListResultModel<RoomTypeDto> RoomTypes);

