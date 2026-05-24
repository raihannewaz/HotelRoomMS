using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Exceptions;
using Dapper;
using FluentValidation;
using HotelRoomMS.Application.Bookings.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Bookings.Features.GetBookingsById;

public record GetBookingById(long Id) : IRequest<GetBookingByIdResponse>;

internal class GetBookingByIdValidator : AbstractValidator<GetBookingById>
{
    public GetBookingByIdValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id is Required");
    }
}

internal class GetBookingByIdHandler : IRequestHandler<GetBookingById, GetBookingByIdResponse>
{
    private readonly IDbConnectionCreator _connectionCreator;

    public GetBookingByIdHandler(IDbConnectionCreator connectionCreator)
    {
        _connectionCreator = connectionCreator;
    }

    public async Task<GetBookingByIdResponse> Handle(GetBookingById request, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT
                                        b.id,
                                        b.bookingNumber,
                                        b.roomId,
                                        r.RoomNumber,
                                        b.CheckIn,
                                        b.ExpectedCheckOut,
                                        b.CheckOut,
                                        b.RoomPrice,
                                        b.TotalAmount,
                                        b.Discount,
                                        b.TotalPaid,
                                        b.Remarks,
                                        b.IsCancelled,
                                        b.IsActive
                                    FROM bookings b
                                    JOIN rooms r on r.id = b.roomid
                                    WHERE b.id = @bookingId
                                    
                                    SELECT
                                        Id,
                                        BookingId,
                                        GuestName,
                                        Relation,
                                        Phone,
                                        Age,
                                        IsPrimary
                                    FROM bookingGuests
                                    WHERE BookingId =@bookingId";

        using var con = _connectionCreator.GetOrCreateConnection();

        var multiQuery = await con.QueryMultipleAsync(sql, new { bookingId = request.Id });

        Guard.Against.Null("Not Found");

        var data = multiQuery.Read<BookingDto>().FirstOrDefault();

        data.BookingGuests = await multiQuery.ReadAsync<BookingGuestDto>();

        return new GetBookingByIdResponse(data);
    }
}