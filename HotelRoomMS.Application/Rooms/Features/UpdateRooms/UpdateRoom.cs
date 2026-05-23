using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.CommonModelProperties;
using FluentValidation;
using HotelRoomMS.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomMS.Application.Rooms.Features.UpdateRooms;

public record UpdateRoom(UpdateRoomRequest Data) : IRequest<CommonResponse>;

internal class UpdateRoomValidator : AbstractValidator<UpdateRoom>
{
    public UpdateRoomValidator()
    {
        RuleFor(x => x.Data.Id).GreaterThan(0).WithMessage("Id Cannot be null or less then 0");
        RuleFor(x => x.Data.HotelId).GreaterThan(0).WithMessage("Hotel Id Cannot be null or less then 0");
        RuleFor(x => x.Data.RoomTypeId).GreaterThanOrEqualTo(0).WithMessage("Hotel Id Cannot be null or less then 0");
        RuleFor(x => x.Data.RoomNumber).NotEmpty().WithMessage("Room number required");
        RuleFor(x => x.Data.PricePerDay).GreaterThanOrEqualTo(0);

    }
}

internal class UpdateRoomHandler : IRequestHandler<UpdateRoom, CommonResponse>
{
    private readonly IDbContext _dbContext;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public UpdateRoomHandler(IDbContext dbContext, ISecurityContextAccessor securityContextAccessor)
    {
        _dbContext = dbContext;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<CommonResponse> Handle(UpdateRoom request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = Convert.ToInt64(_securityContextAccessor.UserId);

            var entity = await _dbContext.Rooms.FirstOrDefaultAsync(x => x.Id == request.Data.Id);
            Guard.Against.Null(entity, nameof(entity), "No Room Found With This Id");

            if (entity.RoomNumber != request.Data.RoomNumber)
            {
                var isExist = await _dbContext.Rooms.AnyAsync(x => x.RoomNumber.ToLower().Trim() == request.Data.RoomNumber.ToLower().Trim() &&
                x.HotelId == request.Data.HotelId, cancellationToken);

                Guard.Against.InvalidInput(request.Data.RoomNumber, nameof(request.Data.RoomNumber), _ => isExist, "A room in this hotel already exists");
            }

            entity.Update(request.Data.HotelId, request.Data.RoomTypeId, request.Data.RoomNumber, request.Data.PricePerDay, userId);

            await _dbContext.SaveChangesAsync();

            return new CommonResponse(true);

        }
        catch (Exception)
        {
            return new CommonResponse(false);
        }
    }
}