using Common.Core.Query;
using HotelRoomMS.Application.Rooms.Dto;

namespace HotelRoomMS.Application.Rooms.Features.GettingRoomsGrid;

public record GettingRoomGridResponse(ListResultModel<RoomDto> Rooms);