using Ardalis.GuardClauses;
using AuthSystem.Identity.Models;
using AuthSystem.Identity.Services;
using AutoMapper;
using Common.Abstractions.CQRS;
using HotelRoomMS.Application.Hotels.Features.GettingHotels;
using HotelRoomMS.Application.Rooms.Features.CreateRooms;
using HotelRoomMS.Application.Rooms.Features.GetRoomsById;
using HotelRoomMS.Application.Rooms.Features.GettingRoomsGrid;
using HotelRoomMS.Application.Rooms.Features.UpdateRooms;
using Microsoft.AspNetCore.Mvc;

namespace HotelRoomMS.Api.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public RoomController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpPost]
        [HasPermission(Permissions.RoomCreate)]
        public async Task<IActionResult> Create(CreateRoomRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new CreateRoom(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPut]
        [HasPermission(Permissions.RoomEdit)]
        public async Task<IActionResult> Edit(UpdateRoomRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new UpdateRoom(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{Id}")]
        [HasPermission(Permissions.RoomView)]
        public async Task<IActionResult> GetById(long Id, CancellationToken cancellationToken)
        {
            Guard.Against.Null(Id, nameof(Id));

            var command = new GetRoomById(Id);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);

        }

        [HttpPost("get/room/grid")]
        [HasPermission(Permissions.HotelView)]
        public async Task<IActionResult> GetAll(GettingRoomGridRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = _mapper.Map<GettingRoomGrid>(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}
