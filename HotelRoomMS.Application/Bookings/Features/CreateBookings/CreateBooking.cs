using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.CommonModelProperties;
using Common.Core.IdsGenerator;
using FluentValidation;
using HotelRoomMS.Domain;
using HotelRoomMS.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomMS.Application.Bookings.Features.CreateBookings;

public record CreateBooking(CreateBookingRequest Data) : IRequest<CommonResponse>;

internal class CreateBookingValidator : AbstractValidator<CreateBooking>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.Data.BookingNumber).NotEmpty().WithMessage("Booking Number is Required");
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

internal class CreateBookingHandler : IRequestHandler<CreateBooking, CommonResponse>
{
    private readonly IDbContext _dbContext;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public CreateBookingHandler(IDbContext dbContext, ISecurityContextAccessor securityContextAccessor)
    {
        _dbContext = dbContext;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<CommonResponse> Handle(CreateBooking request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = Convert.ToInt64(_securityContextAccessor.UserId);

            var isBooked = await _dbContext.Rooms.AnyAsync(x => x.Id == request.Data.RoomId && x.IsBooked == true);
            Guard.Against.InvalidInput(request.Data.RoomId, nameof(request.Data.RoomId), _ => isBooked, "Room Already Booked");

            long primaryId = CurrentDateTimeCountIdGenerator.Id();

            var entity = Booking.Create(
                primaryId,
                request.Data.BookingNumber,
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
                0,
                primaryId,
                x.GuestName ?? "",
                x.Relation ?? "",
                x.Phone ?? "",
                x.Age,
                x.IsPrimary))
                .ToList();


            entity.AddOrUpdateDetails(bookingGuests);


            await _dbContext.Bookings.AddAsync(entity, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CommonResponse(true);

        }
        catch (Exception)
        {

            return new CommonResponse(false);
        }
        
    }
}