using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.CommonModelProperties;
using FluentValidation;
using HotelRoomMS.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomMS.Application.Hotels.Features.UpdateHotels
{
    public record UpdateHotel(UpdateHotelRequest ReqData) : IRequest<CommonResponse>;

    public class UpdateHotelValidator : AbstractValidator<UpdateHotel>
    {
        public UpdateHotelValidator()
        {
            RuleFor(x => x.ReqData.Name).NotEmpty().MaximumLength(100).WithMessage("Name charecter must be between 100");
            RuleFor(x => x.ReqData.Email).EmailAddress().WithMessage("Invalid Email Format");
        }
    }

    public class UpdateHotelHandler : IRequestHandler<UpdateHotel, CommonResponse>
    {
        private readonly IDbContext _dbContext;
        private readonly ISecurityContextAccessor _securityContextAccessor;

        public UpdateHotelHandler(IDbContext dbContext, ISecurityContextAccessor securityContextAccessor)
        {
            _dbContext = dbContext;
            _securityContextAccessor = securityContextAccessor;
        }

        public async Task<CommonResponse> Handle(UpdateHotel request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = Convert.ToInt64(_securityContextAccessor.UserId);

                var hotel = await _dbContext.Hotels.FirstOrDefaultAsync(x => x.Id == request.ReqData.Id);
                Guard.Against.Null(request, nameof(request), "No Data Found With This Id");

                if (hotel?.Name.Trim() != request.ReqData.Name.Trim())
                {
                    var isHotelExist = await _dbContext.Hotels.AnyAsync(x => x.Name.ToLower().Trim() == request.ReqData.Name.ToLower().Trim(), cancellationToken);

                    Guard.Against.InvalidInput(request.ReqData.Name, nameof(request.ReqData.Name), _ => isHotelExist, "A hotel with this name already exists");
                }


                 hotel.Edit(request.ReqData.Name, request.ReqData.Phone ?? "", request.ReqData.Email ?? "", request.ReqData.Address ?? "", userId);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return new CommonResponse(true);
            }
            catch (Exception)
            {
                return new CommonResponse(false);
            }
        }
    }
}