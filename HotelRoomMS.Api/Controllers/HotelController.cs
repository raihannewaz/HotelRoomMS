using Ardalis.GuardClauses;
using AuthSystem.Identity.Models;
using AuthSystem.Identity.Services;
using AutoMapper;
using Common.Abstractions.CQRS;
using HotelRoomMS.Application.Hotels.Features.CreateHotels;
using HotelRoomMS.Application.Hotels.Features.GetHotelsById;
using HotelRoomMS.Application.Hotels.Features.GettingHotels;
using HotelRoomMS.Application.Hotels.Features.UpdateHotels;
using Microsoft.AspNetCore.Mvc;

namespace HotelRoomMS.Api.Controllers
{
    [Route("api/hotel")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public HotelController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }



        [HttpPost]
        [HasPermission(Permissions.HotelCreate)]
        public async Task<IActionResult> Create(CreateHotelRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new CreateHotel(request);

            var result =  await _sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPut]
        [HasPermission(Permissions.HotelEdit)]
        public async Task<IActionResult> Update(UpdateHotelRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new UpdateHotel(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }


        [HttpPost("get/grid")]
        [HasPermission(Permissions.HotelView)]
        public async Task<IActionResult> GetAll(GettingHotelRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = _mapper.Map<GettingHotel>(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{Id}")]
        [HasPermission(Permissions.HotelView)]
        public async Task<IActionResult> GetAll(long Id, CancellationToken cancellationToken)
        {
            Guard.Against.Null(Id, nameof(Id));

            var command = new GetHotelById(Id);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}
