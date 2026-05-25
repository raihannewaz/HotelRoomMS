using Ardalis.GuardClauses;
using AutoMapper;
using Common.Abstractions.CQRS;
using FluentValidation;
using HotelRoomMS.Application.RoomTypes.Dto;
using HotelRoomMS.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.RoomTypes.Features.GetRoomTypesById;

public record GetRoomTypeById(long Id) : IRequest<GetRoomTypeByIdResponse>;

public class GetRoomTypeByIdValidator : AbstractValidator<GetRoomTypeById>
{
    public GetRoomTypeByIdValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}

public class GetRoomTypeByIdHandler : IRequestHandler<GetRoomTypeById, GetRoomTypeByIdResponse>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;
    public GetRoomTypeByIdHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<GetRoomTypeByIdResponse> Handle(GetRoomTypeById request, CancellationToken cancellationToken)
    {
        var data = await _dbContext.RoomTypes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        Guard.Against.Null(data, nameof(data), $"Room type with id {request.Id} not found.");

        var result  = _mapper.Map<RoomTypeDto>(data);

        return new GetRoomTypeByIdResponse(result);
    }
}

