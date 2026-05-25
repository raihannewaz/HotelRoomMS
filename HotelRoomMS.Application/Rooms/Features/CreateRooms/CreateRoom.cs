using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.CommonModelProperties;
using Common.Core.IdsGenerator;
using FluentValidation;
using HotelRoomMS.Domain;
using HotelRoomMS.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Rooms.Features.CreateRooms;

public record CreateRoom(CreateRoomRequest Data) : IRequest<CommonResponse>;

internal class CreateRoomValidator : AbstractValidator<CreateRoom>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.Data.HotelId).GreaterThan(0).WithMessage("Hotel Id Cannot be null or less then 0");
        RuleFor(x => x.Data.RoomTypeId).GreaterThanOrEqualTo(0).WithMessage("Hotel Id Cannot be null or less then 0");
        RuleFor(x => x.Data.RoomNumber).NotEmpty().WithMessage("Room number required");
        RuleFor(x => x.Data.PricePerDay).GreaterThanOrEqualTo(0);
    }
}

internal class CreateRoomHandler : IRequestHandler<CreateRoom, CommonResponse>
{
    private readonly IDbContext _dbContext;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public CreateRoomHandler(IDbContext dbContext, ISecurityContextAccessor securityContextAccessor)
    {
        _dbContext = dbContext;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<CommonResponse> Handle(CreateRoom request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = Convert.ToInt64(_securityContextAccessor.UserId);

            var isEsxis = await _dbContext.Rooms.AnyAsync(x => x.RoomNumber == request.Data.RoomNumber && x.HotelId == request.Data.HotelId);
            Guard.Against.InvalidInput(isEsxis, nameof(isEsxis), _ => !isEsxis, "A Room with the same number in this hotel already exists");

            long primaryId = CurrentDateTimeCountIdGenerator.Id();

            var entity = Room.Create(primaryId, request.Data.HotelId, request.Data.RoomTypeId, request.Data.RoomNumber, request.Data.PricePerDay, userId);
            _dbContext.Rooms.Add(entity);
            await _dbContext.SaveChangesAsync();
            return new CommonResponse(true);
        }
        catch (Exception)
        {

            return new CommonResponse(false);
        }
    }
}