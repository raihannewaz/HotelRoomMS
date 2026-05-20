using AutoMapper;
using HotelRoomMS.Application.Customers.Dto;
using HotelRoomMS.Application.Customers.Features.CreateCustomers;
using HotelRoomMS.Application.Customers.Features.UpdateCustomers;
using HotelRoomMS.Application.RoomTypes.Features.GettingRoomTypes;
using HotelRoomMS.Domain;

namespace HotelRoomMS.Application.Customers
{
    public class CustomerMapper : Profile
    {
        public CustomerMapper()
        {
            CreateMap<Customer, CustomerDto>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
                    .ForMember(x => x.FullName, opt => opt.MapFrom(x => x.FullName))
                    .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address))
                    .ForMember(x => x.Email, opt => opt.MapFrom(x => x.Email))
                    .ForMember(x => x.Phone, opt => opt.MapFrom(x => x.Phone))
                    .ForMember(x => x.NidNumber, opt => opt.MapFrom(x => x.NidNumber))
                    .ForMember(x => x.PassportNumber, opt => opt.MapFrom(x => x.PassportNumber));

            CreateMap<CreateCustomer, Customer>();
            CreateMap<CreateCustomerRequest, CreateCustomer>();

            CreateMap<UpdateCustomer, Customer>();
            CreateMap<UpdateCustomerRequest, UpdateCustomer>();


            CreateMap<GettingRoomTypeRequest, GettingRoomType>();
        }
    }
}
