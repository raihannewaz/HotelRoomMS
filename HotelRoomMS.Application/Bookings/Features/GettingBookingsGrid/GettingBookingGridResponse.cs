using Common.Core.Query;
using HotelRoomMS.Application.Bookings.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Bookings.Features.GettingBookingsGrid;

public record GettingBookingGridResponse(ListResultModel<BookingDto> Bookings);

