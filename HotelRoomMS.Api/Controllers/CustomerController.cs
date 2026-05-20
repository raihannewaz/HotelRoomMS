using AuthSystem.Identity.Models;
using AuthSystem.Identity.Services;
using AutoMapper;
using Common.Abstractions.CQRS;
using HotelRoomMS.Application.Customers.Features.CreateCustomers;
using HotelRoomMS.Application.Customers.Features.GetCustomersById;
using HotelRoomMS.Application.Customers.Features.GettingCustomers;
using HotelRoomMS.Application.Customers.Features.UpdateCustomers;
using Microsoft.AspNetCore.Mvc;

namespace HotelRoomMS.Api.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        public CustomerController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }


        [HttpPost]
        [HasPermission(Permissions.CustomerCreate)]
        public async Task<IActionResult> Create(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateCustomer(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPut]
        [HasPermission(Permissions.CustomerEdit)]
        public async Task<IActionResult> Update(UpdateCustomerRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateCustomer(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.CustomerView)]
        public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
        {
            var query = new GetCustomerById(id);

            var result = await _sender.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpPost("get/customers/grid")]
        [HasPermission(Permissions.CustomerView)]
        public async Task<IActionResult> GetAll(GettingCustomerRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<GettingCustomer>(request);

            var result = await _sender.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}
