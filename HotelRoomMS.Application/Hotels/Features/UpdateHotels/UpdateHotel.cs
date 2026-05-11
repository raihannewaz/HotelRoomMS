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
        private readonly IDbContext _dbCOntext;
        private readonly ISecurityContextAccessor _securityContextAccessor;

        public UpdateHotelHandler(IDbContext dbCOntext, ISecurityContextAccessor securityContextAccessor)
        {
            _dbCOntext = dbCOntext;
            _securityContextAccessor = securityContextAccessor;
        }

        public async Task<CommonResponse> Handle(UpdateHotel reequest, CancellationToken cancellationToken)
        {
            try
            {
                var userId = Convert.ToInt64(_securityContextAccessor.UserId);

                var hotel = await _dbCOntext.Hotels.FirstOrDefaultAsync(x => x.Id == reequest.ReqData.Id);
                Guard.Against.Null(reequest, nameof(reequest), "No Data Found With This Id");

                if (hotel?.Name.Trim() != reequest.ReqData.Name.Trim())
                {
                    var isHotelExist = _dbCOntext.Hotels.Any(x => x.Name.Trim() == reequest.ReqData.Name.Trim());
                    if (isHotelExist)
                        Guard.Against.InvalidInput(reequest.ReqData.Name, nameof(reequest.ReqData.Name), _ => isHotelExist, "ReqData with the same name already exists");
                }


                 hotel.Edit(reequest.ReqData.Name, reequest.ReqData.Phone ?? "", reequest.ReqData.Email ?? "", reequest.ReqData.Address ?? "", userId);

                await _dbCOntext.SaveChangesAsync(cancellationToken);

                return new CommonResponse(true);
            }
            catch (Exception)
            {
                return new CommonResponse(false);
            }
        }
    }
}