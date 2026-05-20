using HotelRoomMS.Application.Customers.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Customers.Features.GetCustomersById;

public record GetCustomerByIdResponse(CustomerDto Customers);

