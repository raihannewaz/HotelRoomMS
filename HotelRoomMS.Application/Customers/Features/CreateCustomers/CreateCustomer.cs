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

namespace HotelRoomMS.Application.Customers.Features.CreateCustomers
{
    public record CreateCustomer(CreateCustomerRequest ReqData) : IRequest<CommonResponse>;

    public class CreateCustomerValidator : AbstractValidator<CreateCustomer>
    {
        public CreateCustomerValidator()
        {
            RuleFor(x => x.ReqData.FullName).NotEmpty().WithMessage("FullName is required.")
                .MaximumLength(100).WithMessage("FullName must not exceed 100 characters.");
            RuleFor(x => x.ReqData.Phone).MaximumLength(20).WithMessage("Phone must not exceed 20 characters.");
            RuleFor(x => x.ReqData.Email).MaximumLength(50).WithMessage("Email must not exceed 50 characters.");
            RuleFor(x => x.ReqData.NidNumber).MaximumLength(20).WithMessage("NidNumber must not exceed 20 characters.");
            RuleFor(x => x.ReqData.PassportNumber).MaximumLength(20).WithMessage("PassportNumber must not exceed 20 characters.");
            RuleFor(x => x.ReqData.Address).MaximumLength(150).WithMessage("Address must not exceed 150 characters.");
        }
    }

    public class CreateCustomerHandler : IRequestHandler<CreateCustomer, CommonResponse>
    {
        private readonly IDbContext _dbContext;
        private readonly ISecurityContextAccessor _securityContextAccessor;
        public CreateCustomerHandler(IDbContext dbContext, ISecurityContextAccessor securityContextAccessor)
        {
            _dbContext = dbContext;
            _securityContextAccessor = securityContextAccessor;
        }
        public async Task<CommonResponse> Handle(CreateCustomer request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = Convert.ToInt64(_securityContextAccessor.UserId);

                var existingCustomer = await _dbContext.Customers
                    .Where(x => x.Phone == request.ReqData.Phone
                             || x.Email == request.ReqData.Email
                             || x.NidNumber == request.ReqData.NidNumber
                             || x.PassportNumber == request.ReqData.PassportNumber)
                    .Select(x => new { x.Phone, x.Email, x.NidNumber, x.PassportNumber })
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingCustomer != null)
                {
                    string reason = existingCustomer.Email == request.ReqData.Email ? "Email" :
                                    existingCustomer.Phone == request.ReqData.Phone ? "Phone" :
                                    existingCustomer.NidNumber == request.ReqData.NidNumber ? "NID" : "Passport";

                    Guard.Against.InvalidInput(request.ReqData, nameof(request.ReqData), _ => true, $"A customer with this {reason} already exists.");
                }

                var entity = Customer.Create(request.ReqData.FullName, request.ReqData.Phone, request.ReqData.Email, request.ReqData.NidNumber, request.ReqData.PassportNumber, request.ReqData.Address, userId);

                _dbContext.Customers.Add(entity);

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

