using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Bookings.Features.GetBookingNumbers;

public record GetBookingNumber : IRequest<GetBookingNumberResponse>;

internal class GetBookingNumberHandler : IRequestHandler<GetBookingNumber, GetBookingNumberResponse>
{
    private readonly IDbConnectionCreator _connectionCreator;

    public GetBookingNumberHandler(IDbConnectionCreator connectionCreator)
    {
        _connectionCreator = connectionCreator;
    }

    public async Task<GetBookingNumberResponse> Handle(GetBookingNumber request, CancellationToken cancellationToken)
    {
		using (var con = _connectionCreator.GetOrCreateConnection())
		{
			string bookingNumber = await con.QueryFirstAsync<string>("SELECT dbo.[fn_generate_booking_no]() ;");
            Guard.Against.NullOrEmpty(bookingNumber);

            return new GetBookingNumberResponse(bookingNumber);
		}
    }
}