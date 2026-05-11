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

            CreateMap<CreateHotel, Hotel>();
            CreateMap<CreateHotelRequest, CreateHotel>();
            //.ConstructUsing(req => new CreateHotel(req));

            CreateMap<UpdateHotel, Hotel>();
            CreateMap<UpdateHotelRequest, UpdateHotel>();
            //.ConstructUsing(req => new UpdateHotel(req));

            CreateMap<GettingHotelRequest, GettingHotel>();
        }

        //public static BookDto QueryResponse(Book book)
        //{
        //    return new BookDto
        //    {
        //        Id = book.Id,
        //        Title = book.Title,
        //        PublishedMonth = book.PublishedMonth,
        //        PublicationName = book.PublicationName,
        //        AutherName = book.AutherName,
        //        CoverImage = book.CoverImage,
        //        IsAvailable = book.IsAvailable

        //    };
        //}

        //public static Category CreateWithDto(CategoryDto dto)
        //{
        //    return Category.Create(dto.ParentId, dto.CategoryName);
        //}

        //public static void UpdateWithDto(Category category, CategoryDto dto)
        //{
        //    category.Update(dto.ParentId, dto.CategoryName);
        //}

        //public static GettingHotel GetRequestMap(GettingHotelRequest request)
        //{
        //    return new GettingBookGrid()
        //    {
        //        Includes = request.Includes,
        //        Filters = request.Filters,
        //        Sorts = request.Sorts,
        //        Page = request.Page,
        //        PageSize = request.PageSize
        //    };
        //}

    }
}
