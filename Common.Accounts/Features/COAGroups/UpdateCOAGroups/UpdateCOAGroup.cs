using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Accounts.Data;
using Common.Core.CommonModelProperties;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Features.COAGroups.UpdateCOAGroups
{
    public record UpdateCOAGroup(UpdateCOAGroupRequest ReqData) : IRequest<CommonResponse>;
    
    internal class UpdateCOAGroupValidator : AbstractValidator<UpdateCOAGroup>
    {
        public UpdateCOAGroupValidator()
        {
            RuleFor(x => x.ReqData.Id).GreaterThan(0);
            RuleFor(x => x.ReqData.ParentId).GreaterThanOrEqualTo(0).WithMessage("ParentId must be greater than or equal to 0.");
            RuleFor(x => x.ReqData.Name).NotEmpty().MaximumLength(100);
        }
    }

    internal class UpdateCOAGroupHandler : IRequestHandler<UpdateCOAGroup, CommonResponse>
    {
        private readonly AccountsDbContext _dbContext;
        private readonly ISecurityContextAccessor _securityContextAccessor;

        public UpdateCOAGroupHandler(AccountsDbContext dbContext, ISecurityContextAccessor securityContextAccessor)
        {
            _dbContext = dbContext;
            _securityContextAccessor = securityContextAccessor;
        }

        public async Task<CommonResponse> Handle(UpdateCOAGroup request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = Convert.ToInt64(_securityContextAccessor.UserId);

                var entity = await _dbContext.Groups.FirstOrDefaultAsync(c => c.Id == request.ReqData.Id, cancellationToken);
                Guard.Against.Null(entity, "COA Group not found.");

                var conflict = await _dbContext.Groups
                    .Where(x => x.Id != request.ReqData.Id)
                    .Where(x => x.Name == request.ReqData.Name)
                    .Select(x => new { x.Name })
                    .FirstOrDefaultAsync(cancellationToken);

                if (conflict != null)
                {
                    var field = conflict.Name == request.ReqData.Name ? "Name" : "";

                    Guard.Against.InvalidInput(request.ReqData.Name, nameof(request.ReqData.Name), _ => true,
                        $"The {field} is already used.");
                }

                entity.Update(request.ReqData.ParentId, request.ReqData.Name, userId);

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
