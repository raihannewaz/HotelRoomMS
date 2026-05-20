using Common.Abstractions.CQRS;
using Common.Accounts.Data;
using Common.Core.CommonModelProperties;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Features.FiscalYears.CreateFiscalYears;

public record CreateFiscalYear(CreateFiscalYearRequest Data) : IRequest<CommonResponse>;

public class CreateFiscalYearValidator : AbstractValidator<CreateFiscalYear>
{
    public CreateFiscalYearValidator()
    {
        RuleFor(x => x.Data.FromDate).NotEmpty().WithMessage("From Date is required");
    }
}

public class CreateFiscalYearHandler : IRequestHandler<CreateFiscalYear, CommonResponse>
{
    private readonly AccountsDbContext _context;    
    public Task<CommonResponse> Handle(CreateFiscalYear reequest, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}