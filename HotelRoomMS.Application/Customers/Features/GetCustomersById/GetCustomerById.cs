using Ardalis.GuardClauses;
using AutoMapper;
using Common.Abstractions.CQRS;
using FluentValidation;
using HotelRoomMS.Application.Customers.Dto;
using HotelRoomMS.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomMS.Application.Customers.Features.GetCustomersById;

public record GetCustomerById(long Id) : IRequest<GetCustomerByIdResponse>;

public class GetCustomerByIdValidator : AbstractValidator<GetCustomerById>
{
    public GetCustomerByIdValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}

public class GetCustomerByIdHandler : IRequestHandler<GetCustomerById, GetCustomerByIdResponse>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;
    public GetCustomerByIdHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<GetCustomerByIdResponse> Handle(GetCustomerById request, CancellationToken cancellationToken)
    {
        var data = await _dbContext.Customers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        Guard.Against.Null(data, nameof(data));

        var result = _mapper.Map<CustomerDto>(data);

        return new GetCustomerByIdResponse(result);
    }
}
