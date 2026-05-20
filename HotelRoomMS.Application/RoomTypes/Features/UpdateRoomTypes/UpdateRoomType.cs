using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Core.CommonModelProperties;
using FluentValidation;
using HotelRoomMS.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.RoomTypes.Features.UpdateRoomTypes;

public record UpdateRoomType(UpdateRoomTypeRequest ReqData) : IRequest<CommonResponse>;

public class UpdateRoomTypeValidator : AbstractValidator<UpdateRoomType>
{
    public UpdateRoomTypeValidator()
    {
        RuleFor(x => x.ReqData.Id).GreaterThan(0).WithMessage("Id must be greater than 0.");
        RuleFor(x => x.ReqData.Name).NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        RuleFor(x => x.ReqData.BasePrice).GreaterThanOrEqualTo(0).WithMessage("Base price must be a non-negative value.");
    }
}

public class UpdateRoomTypeHandler : IRequestHandler<UpdateRoomType, CommonResponse>
{
    private readonly IDbContext _dbContext;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public UpdateRoomTypeHandler(IDbContext dbContext, ISecurityContextAccessor securityContextAccessor)
    {
        _dbContext = dbContext;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<CommonResponse> Handle(UpdateRoomType request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = Convert.ToInt64(_securityContextAccessor.UserId);

            var entity = await _dbContext.RoomTypes.FirstOrDefaultAsync(c => c.Id == request.ReqData.Id, cancellationToken);
            Guard.Against.Null(entity, "Room type not found.");

            var conflict = await _dbContext.RoomTypes.Where(x => x.Id != request.ReqData.Id).Where(x => x.Name == request.ReqData.Name).Select(x => new { x.Name}).FirstOrDefaultAsync(cancellationToken);

            if (conflict != null)
            {
                // Check which specific field is the problem
                var field = conflict.Name == request.ReqData.Name ? "Name" : "Unknown";

                Guard.Against.InvalidInput(request.ReqData, nameof(request.ReqData), _ => true,$"{field} already exist.");
            }

            entity.Update(request.ReqData.Name, request.ReqData.BasePrice, userId);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CommonResponse(true);
        }
        catch (Exception)
        {

            throw;
        }
    }
}
