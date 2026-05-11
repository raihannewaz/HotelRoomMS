using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.CommonModelProperties;
using Common.Core.IdsGenerator;
using FluentValidation;
using HotelRoomMS.Domain;
using HotelRoomMS.Infrastructure.DbContexts;

namespace HotelRoomMS.Application.Hotels.Features.CreateHotels
{
    public record CreateHotel(CreateHotelRequest ReqData) : IRequest<CommonResponse>;

    public class CreateHotelValidator : AbstractValidator<CreateHotel>
    {
        public CreateHotelValidator()
        {
            RuleFor(x => x.ReqData.Name).NotEmpty().MaximumLength(100).WithMessage("Name charecter must be between 100");
            RuleFor(x => x.ReqData.Email).EmailAddress().WithMessage("Invalid Email Format");
        }
    }

    public class CreateHotelHandler : IRequestHandler<CreateHotel, CommonResponse>
    {
        private readonly IDbContext _dbCOntext;
        private readonly ISecurityContextAccessor _securityContextAccessor;

        public CreateHotelHandler(IDbContext dbCOntext, ISecurityContextAccessor securityContextAccessor)
        {
            _dbCOntext = dbCOntext;
            _securityContextAccessor = securityContextAccessor;
        }

        public async Task<CommonResponse> Handle(CreateHotel reequest, CancellationToken cancellationToken)
        {
            try
            {
                var userId = Convert.ToInt64(_securityContextAccessor.UserId);

                var isHotelExist = _dbCOntext.Hotels.Any(x => x.Name.Trim() == reequest.ReqData.Name.Trim());
                if (isHotelExist)
                    Guard.Against.InvalidInput(reequest.ReqData.Name, nameof(reequest.ReqData.Name), _ => isHotelExist, "ReqData with the same name already exists");

                long PrimaryId = CurrentDateTimeCountIdGenerator.Id();

                var hotel = Hotel.Create(PrimaryId, reequest.ReqData.Name, reequest.ReqData.Phone ?? "", reequest.ReqData.Email ?? "", reequest.ReqData.Address ?? "", userId);

                await _dbCOntext.Hotels.AddAsync(hotel);

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
