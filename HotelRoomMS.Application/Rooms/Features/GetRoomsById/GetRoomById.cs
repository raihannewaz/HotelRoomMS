using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Dapper;
using FluentValidation;
using HotelRoomMS.Application.Rooms.Dto;

namespace HotelRoomMS.Application.Rooms.Features.GetRoomsById;

public record GetRoomById(long Id) : IRequest<GetRoomByIdResponse>;

internal class GetRoomByIdValidator : AbstractValidator<GetRoomById>
{
    public GetRoomByIdValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id Is Required");
    }
}

internal class GetRoomByIdHandler : IRequestHandler<GetRoomById, GetRoomByIdResponse>
{
    private readonly IDbConnectionCreator _dbConnection;

    public GetRoomByIdHandler(IDbConnectionCreator dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<GetRoomByIdResponse> Handle(GetRoomById request, CancellationToken cancellationToken)
    {
        using (var con = _dbConnection.GetOrCreateConnection())
        {
            const string sql = @"SELECT r.Id,
                                        r.HotelId,
                                        h.name as HotelName,
                                        r.RoomTypeId,
                                        rt.name as RoomType,
                                        r.RoomNumber,
                                        r.PricePerDay,
                                        r.IsBooked,
                                        r.IsActive 
                                 FROM rooms r 
                                 Join hotels h on r.hotelId = h.id 
                                 Join roomtypes rt on r.roomTypeId = rt.id 
                                 WHERE r.Id = @roomId ";

            var result = await con.QueryFirstOrDefaultAsync<RoomDto>(sql, new { roomId = request.Id});
            Guard.Against.Null(result, nameof(result), "No Room Found With This Id");


            return new GetRoomByIdResponse(result);
        }
    }
}