using AutoMapper;
using Common.Abstractions.CQRS;
using HotelRoomMS.Application.RoomTypes.Features.CreateRoomTypes;
using HotelRoomMS.Application.RoomTypes.Features.GetRoomTypesById;
using HotelRoomMS.Application.RoomTypes.Features.GettingRoomTypes;
using HotelRoomMS.Application.RoomTypes.Features.UpdateRoomTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelRoomMS.Api.Controllers
{
    [Route("api/roomTypes")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public RoomTypeController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoomTypeRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateRoomType(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateRoomTypeRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateRoomType(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
        {
            var query = new GetRoomTypeById(id);

            var result = await _sender.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpPost("get/roomTypes/grid")]
        public async Task<IActionResult> GetRoomTypesGrid(GettingRoomTypeRequest request, CancellationToken cancellationToken)
        {
            
            var query = _mapper.Map<GettingRoomType>(request);

            var result = await _sender.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
