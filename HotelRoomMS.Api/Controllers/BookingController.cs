using Ardalis.GuardClauses;
using AuthSystem.Identity.Models;
using AuthSystem.Identity.Services;
using AutoMapper;
using Common.Abstractions.CQRS;
using HotelRoomMS.Application.Bookings.Features.CreateBookings;
using HotelRoomMS.Application.Bookings.Features.GetBookingsById;
using HotelRoomMS.Application.Bookings.Features.GettingBookingsGrid;
using HotelRoomMS.Application.Bookings.Features.UpdateBookigs;
using HotelRoomMS.Application.Rooms.Features.CreateRooms;
using HotelRoomMS.Application.Rooms.Features.GetRoomsById;
using HotelRoomMS.Application.Rooms.Features.GettingRoomsGrid;
using HotelRoomMS.Application.Rooms.Features.UpdateRooms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelRoomMS.Api.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public BookingController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpPost]
        [HasPermission(Permissions.BookingCreate)]
        public async Task<IActionResult> Create(CreateBookingRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new CreateBooking(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPut]
        [HasPermission(Permissions.BookingEdit)]
        public async Task<IActionResult> Edit(UpdateBookingRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new UpdateBooking(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{Id}")]
        [HasPermission(Permissions.BookingView)]
        public async Task<IActionResult> GetById(long Id, CancellationToken cancellationToken)
        {
            Guard.Against.Null(Id, nameof(Id));

            var command = new GetBookingById(Id);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);

        }

        [HttpPost("get/grid")]
        [HasPermission(Permissions.BookingView)]
        public async Task<IActionResult> GetAll(GettingBookingGridRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = _mapper.Map<GettingBookingGrid>(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}
