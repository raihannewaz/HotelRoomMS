using HotelRoomMS.Application.Bookings.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Bookings.Features.GetBookingsById;

public record GetBookingByIdResponse(BookingDto Booking);

