using AutoMapper;
using HotelRoomMS.Application.Bookings.Dto;
using HotelRoomMS.Application.Bookings.Features.CreateBookings;
using HotelRoomMS.Application.Bookings.Features.GettingBookingsGrid;
using HotelRoomMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Bookings
{
    internal class BookingMapper : Profile
    {
        public BookingMapper()
        {
            CreateMap<Booking, BookingDto>();

            CreateMap<GettingBookingGridRequest, GettingBookingGrid>();
        }
    }
    
}
