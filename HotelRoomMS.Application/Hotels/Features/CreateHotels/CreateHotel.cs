using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.CommonModelProperties;
using Common.Core.IdsGenerator;
using FluentValidation;
using HotelRoomMS.Domain;
using HotelRoomMS.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

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
        private readonly IDbContext _dbContext;
        private readonly ISecurityContextAccessor _securityContextAccessor;

        public CreateHotelHandler(IDbContext dbContext, ISecurityContextAccessor securityContextAccessor)
        {
            _dbContext = dbContext;
            _securityContextAccessor = securityContextAccessor;
        }

        public async Task<CommonResponse> Handle(CreateHotel request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = Convert.ToInt64(_securityContextAccessor.UserId);


                var isHotelExist = await _dbContext.Hotels.AnyAsync(x => x.Name.ToLower().Trim() == request.ReqData.Name.ToLower().Trim(), cancellationToken);

                Guard.Against.InvalidInput(request.ReqData.Name, nameof(request.ReqData.Name), _ => !isHotelExist,"A hotel with this name already exists");


                long PrimaryId = CurrentDateTimeCountIdGenerator.Id();

                var hotel = Hotel.Create(PrimaryId, request.ReqData.Name, request.ReqData.Phone ?? "", request.ReqData.Email ?? "", request.ReqData.Address ?? "", userId);

                _dbContext.Hotels.Add(hotel);

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
