using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Accounts.Data;
using Common.Accounts.Models;
using Common.Core.CommonModelProperties;
using Common.Core.IdsGenerator;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Common.Accounts.Features.COAGroups.CreateCOAGroups
{
    public record CreateCOAGroup(CreateCOAGroupRequest ReqData) : IRequest<CommonResponse>;

    public class CreateCOAGroupValidator : AbstractValidator<CreateCOAGroup>
    {
        public CreateCOAGroupValidator()
        {
            RuleFor(x => x.ReqData.ParentId).GreaterThanOrEqualTo(0).WithMessage("ParentId must be greater than or equal to 0.");
            RuleFor(x => x.ReqData.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(100);
        }
    }

    internal class CreateCOAGroupHandler : IRequestHandler<CreateCOAGroup, CommonResponse>
    {
        private readonly AccountsDbContext _context;
        private readonly ISecurityContextAccessor _securityContextAccessor;

        public CreateCOAGroupHandler(AccountsDbContext context, ISecurityContextAccessor securityContextAccessor)
        {
            _context = context;
            _securityContextAccessor = securityContextAccessor;
        }

        public async Task<CommonResponse> Handle(CreateCOAGroup request, CancellationToken cancellationToken)
        {
            try
            {
                long userId = Convert.ToInt64(_securityContextAccessor.UserId);

                long primaryId = CurrentDateTimeCountIdGenerator.Id();

                var existingCOAGroup = await _context.Groups.Where(x => x.Name == request.ReqData.Name).Select(x => new { x.Name })
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingCOAGroup != null)
                {
                    string reason = nameof(request.ReqData.Name);

                    Guard.Against.InvalidInput(request.ReqData.Name, nameof(request.ReqData.Name), _ => true, $"A Group with this {reason} already exists.");

                }

                var coaGroup = COAGroup.Create(primaryId, request.ReqData.ParentId, request.ReqData.Name, userId);

                 _context.Groups.Add(coaGroup);

                await _context.SaveChangesAsync(cancellationToken);


                return new CommonResponse(true);
            }
            catch (Exception)
            {

                return new CommonResponse(false);
            }
        }
    }
}
