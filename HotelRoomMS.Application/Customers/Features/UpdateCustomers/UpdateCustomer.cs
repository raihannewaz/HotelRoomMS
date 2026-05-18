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

namespace HotelRoomMS.Application.Customers.Features.UpdateCustomers
{
    public record UpdateCustomer(UpdateCustomerRequest ReqData) : IRequest<CommonResponse>;

    public class UpdateCustomerValidator : AbstractValidator<UpdateCustomer>
    {
        public UpdateCustomerValidator()
        {
            RuleFor(x => x.ReqData.Id).GreaterThan(0).WithMessage("Id must be greater than 0.");
            RuleFor(x => x.ReqData.FullName).NotEmpty().WithMessage("FullName is required.")
                .MaximumLength(100).WithMessage("FullName must not exceed 100 characters.");
            RuleFor(x => x.ReqData.Phone).MaximumLength(20).WithMessage("Phone must not exceed 20 characters.");
            RuleFor(x => x.ReqData.Phone).NotEmpty().Matches(@"^\+[1-9]\d{1,14}$").WithMessage("Please enter a valid phone number including the country code (e.g., +1234567890).");
            RuleFor(x => x.ReqData.Email).MaximumLength(50).WithMessage("Email must not exceed 50 characters.");
            RuleFor(x => x.ReqData.NidNumber).MaximumLength(20).WithMessage("NidNumber must not exceed 20 characters.");
            RuleFor(x => x.ReqData.PassportNumber).MaximumLength(20).WithMessage("PassportNumber must not exceed 20 characters.");
            RuleFor(x => x.ReqData.Address).MaximumLength(150).WithMessage("Address must not exceed 150 characters.");
        }
    }

    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomer, CommonResponse>
    {
        private readonly IDbContext _dbContext;
        private readonly ISecurityContextAccessor _securityContextAccessor;
        public UpdateCustomerHandler(IDbContext dbContext, ISecurityContextAccessor securityContextAccessor)
        {
            _dbContext = dbContext;
            _securityContextAccessor = securityContextAccessor;
        }
        public async Task<CommonResponse> Handle(UpdateCustomer request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = Convert.ToInt64(_securityContextAccessor.UserId);

                var entity = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == request.ReqData.Id, cancellationToken);
                Guard.Against.Null(entity, "Customer not found.");

                // 2. Check for conflicts with OTHER people
                var conflict = await _dbContext.Customers
                    .Where(x => x.Id != request.ReqData.Id) // "Check everyone except the current user"
                    .Where(x => x.Phone == request.ReqData.Phone
                             || x.Email == request.ReqData.Email
                             || x.NidNumber == request.ReqData.NidNumber
                             || x.PassportNumber == request.ReqData.PassportNumber)
                    .Select(x => new { x.Phone, x.Email, x.NidNumber, x.PassportNumber })
                    .FirstOrDefaultAsync(cancellationToken);

                if (conflict != null)
                {
                    // Check which specific field is the problem
                    var field = conflict.Phone == request.ReqData.Phone ? "Phone" :
                                conflict.Email == request.ReqData.Email ? "Email" :
                                conflict.NidNumber == request.ReqData.NidNumber ? "NID" : "Passport";

                    Guard.Against.InvalidInput(request.ReqData, nameof(request.ReqData), _ => true,
                        $"The {field} provided is already assigned to another customer.");
                }

                entity.Update(request.ReqData.FullName, request.ReqData.Phone, request.ReqData.Email, request.ReqData.NidNumber, request.ReqData.PassportNumber, request.ReqData.Address, userId);

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
