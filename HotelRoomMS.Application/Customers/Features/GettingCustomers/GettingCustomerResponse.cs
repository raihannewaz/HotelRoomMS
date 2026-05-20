using Common.Core.Query;
using HotelRoomMS.Application.Customers.Dto;

namespace HotelRoomMS.Application.Customers.Features.GettingCustomers;

public record GettingCustomerResponse(ListResultModel<CustomerDto> Customers);
