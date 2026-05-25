using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.CommonModelProperties;
using FluentValidation;
using HotelRoomMS.Domain;
using HotelRoomMS.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomMS.Application.Bookings.Features.UpdateBookigs;

public record UpdateBooking(UpdateBookingRequest Data) : IRequest<CommonResponse>;

internal class UpdateBookingValidator : AbstractValidator<UpdateBooking>
{
    public UpdateBookingValidator()
    {
        RuleFor(x => x.Data.RoomId).GreaterThan(0).WithMessage("Room number is Required");
        RuleFor(x => x.Data.CheckIn).NotNull().WithMessage("Check In Date is Required");
        RuleFor(x => x.Data.RoomPrice).GreaterThanOrEqualTo(0).WithMessage("Can't be Negative Value");
        RuleFor(x => x.Data.Discount).GreaterThanOrEqualTo(0).WithMessage("Can't be Negative Value");
        RuleFor(x => x.Data.CustomerId).GreaterThanOrEqualTo(0).WithMessage("Can't be Negative Value");
        RuleFor(x => x.Data.TotalAmount).GreaterThanOrEqualTo(0).WithMessage("Can't be Negative Value");
        RuleFor(x => x.Data.TotalPaid).GreaterThanOrEqualTo(0).WithMessage("Can't be Negative Value");
        RuleFor(x => x.Data.GuestRequests).NotNull().WithMessage("Can't be NUll");
    }
}

internal class UpdateBookingHandler : IRequestHandler<UpdateBooking, CommonResponse>
{
    private readonly IDbContext _dbContext;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public UpdateBookingHandler(IDbContext dbContext, ISecurityContextAccessor securityContextAccessor)
    {
        _dbContext = dbContext;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<CommonResponse> Handle(UpdateBooking request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = Convert.ToInt64(_securityContextAccessor.UserId);

            var existingData = await _dbContext.Bookings.Include(x => x.BookingGuests).FirstOrDefaultAsync(x => x.Id == request.Data.Id, cancellationToken);


            Guard.Against.Null(existingData);

            if (existingData.RoomId != request.Data.RoomId)
            {
                var isBooked = await _dbContext.Rooms.AnyAsync(x => x.Id == request.Data.RoomId && x.IsBooked == true);
                Guard.Against.InvalidInput(request.Data.RoomId, nameof(request.Data.RoomId), _ => !isBooked, "Room Already Booked");
            }

            existingData.Update(
                request.Data.CustomerId,
                request.Data.RoomId,
                request.Data.CheckIn,
                request.Data.ExpectedCheckOut,
                request.Data.RoomPrice,
                request.Data.Remarks,
                request.Data.TotalAmount,
                request.Data.TotalPaid,
                userId);


            var bookingGuests = request.Data.GuestRequests.Select(x => BookingGuest.Create(
                x.Id,
                request.Data.Id,
                x.GuestName ?? "",
                x.Relation ?? "",
                x.Phone ?? "",
                x.Age,
                x.IsPrimary))
                .ToList();

            existingData.AddOrUpdateDetails(bookingGuests);

            await _dbContext.SaveChangesAsync();

            return new CommonResponse(true);


        }
        catch (Exception)
        {
            return new CommonResponse(false);
        }
    }
}