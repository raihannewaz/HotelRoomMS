using AutoMapper;
using HotelRoomMS.Application.RoomTypes.Dto;
using HotelRoomMS.Application.RoomTypes.Features.CreateRoomTypes;
using HotelRoomMS.Application.RoomTypes.Features.GettingRoomTypes;
using HotelRoomMS.Application.RoomTypes.Features.UpdateRoomTypes;
using HotelRoomMS.Domain;

namespace HotelRoomMS.Application.RoomTypes
{
    internal class RoomTypeMapper : Profile
    {
        public RoomTypeMapper()
        {
            CreateMap<RoomType, RoomTypeDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
                    .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name))
                    .ForMember(x => x.BasePrice, opt => opt.MapFrom(x => x.BasePrice))
                    .ForMember(x => x.IsActive, opt => opt.MapFrom(x => x.IsActive));


            CreateMap<GettingRoomTypeRequest, GettingRoomType>();
        }
    }
}
