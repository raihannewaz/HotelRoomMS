using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.CommonModelProperties;
using FluentValidation;
using HotelRoomMS.Domain;
using HotelRoomMS.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.RoomTypes.Features.CreateRoomTypes
{
    public record CreateRoomType(CreateRoomTypeRequest ReqData) : IRequest<CommonResponse>;

    public class CreateRoomTypeValidator : AbstractValidator<CreateRoomType>
    {
        public CreateRoomTypeValidator()
        {
            RuleFor(x => x.ReqData.Name).NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
            RuleFor(x => x.ReqData.BasePrice).GreaterThanOrEqualTo(0).WithMessage("Base price must be a non-negative value.");
        }
    }

    public class CreateRoomTypeHandler : IRequestHandler<CreateRoomType, CommonResponse>
    {
        private readonly IDbContext _dbContext;
        private readonly ISecurityContextAccessor _securityContextAccessor;

        public CreateRoomTypeHandler(IDbContext dbContext, ISecurityContextAccessor securityContextAccessor)
        {
            _dbContext = dbContext;
            _securityContextAccessor = securityContextAccessor;
        }

        public async Task<CommonResponse> Handle(CreateRoomType request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = Convert.ToInt64(_securityContextAccessor.UserId);

                var existing = await _dbContext.RoomTypes.Where(x => x.Name == request.ReqData.Name).Select(x => new { x.Name })
                    .FirstOrDefaultAsync(cancellationToken);

                if (existing != null)
                {
                    string reason = existing.Name == request.ReqData.Name ? "Name" : "Unknown";

                    Guard.Against.InvalidInput(request.ReqData, nameof(request.ReqData), _ => true, $"{reason} already exists.");
                }

                var roomType = RoomType.Create(request.ReqData.Name, request.ReqData.BasePrice, userId);

                await _dbContext.RoomTypes.AddAsync(roomType, cancellationToken);

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
