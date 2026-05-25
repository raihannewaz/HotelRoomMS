using AutoMapper;
using HotelRoomMS.Application.Hotels.Dto;
using HotelRoomMS.Application.Hotels.Features.CreateHotels;
using HotelRoomMS.Application.Hotels.Features.GettingHotels;
using HotelRoomMS.Application.Hotels.Features.UpdateHotels;
using HotelRoomMS.Domain;

namespace HotelRoomMS.Application.Hotels
{
    public class HotelsMapper : Profile
    {

        public HotelsMapper()
        {
            CreateMap<Hotel, HotelDto>()
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address))
                .ForMember(x => x.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(x => x.Phone, opt => opt.MapFrom(x => x.Phone))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(x => x.IsActive));


            CreateMap<GettingHotelRequest, GettingHotel>();
        }
    }
}
