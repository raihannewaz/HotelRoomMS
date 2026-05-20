using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Abstractions.CQRS;
using FluentValidation;
using HotelRoomMS.Application.Hotels.Dto;
using HotelRoomMS.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomMS.Application.Hotels.Features.GetHotelsById
{
    public record GetHotelById(long Id) : IRequest<GetHotelByIdResponse>;

    internal class GetHotelByIdValidator : AbstractValidator<GetHotelById>
    {
        public GetHotelByIdValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    public class GetHotelByIdlHandler : IRequestHandler<GetHotelById, GetHotelByIdResponse>
    {
        private readonly IDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetHotelByIdlHandler(IDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<GetHotelByIdResponse> Handle(GetHotelById request, CancellationToken cancellationToken)
        {
            var data = await _dbContext.Hotels.FirstOrDefaultAsync(x => x.Id == request.Id);

            Guard.Against.Null(data, nameof(data));

            var result = _mapper.Map<HotelDto>(data);

            return new GetHotelByIdResponse(result);

        }
    }
}
