using AutoMapper;
using HotelRoomMS.Application.Rooms.Dto;
using HotelRoomMS.Application.Rooms.Features.CreateRooms;
using HotelRoomMS.Domain;

namespace HotelRoomMS.Application.Rooms
{
    internal class RoomMapper : Profile
    {
        public RoomMapper()
        {
            CreateMap<Room, RoomDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
                    .ForMember(x => x.RoomNumber, opt => opt.MapFrom(x => x.RoomNumber))
                    .ForMember(x => x.HotelId, opt => opt.MapFrom(x => x.HotelId))
                    .ForMember(x => x.RoomTypeId, opt => opt.MapFrom(x => x.RoomTypeId))
                    .ForMember(x => x.PricePerDay, opt => opt.MapFrom(x => x.PricePerDay))
                    .ForMember(x => x.IsBooked, opt => opt.MapFrom(x => x.IsBooked))
                    .ForMember(x => x.IsActive, opt => opt.MapFrom(x => x.IsActive));

            CreateMap<CreateRoom, Customer>();
            CreateMap<CreateRoomRequest, CreateRoom>();

        }
    }
}
