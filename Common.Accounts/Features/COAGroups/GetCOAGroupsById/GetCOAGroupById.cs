using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Accounts.Data;
using Common.Accounts.DTOs;
using Common.Accounts.Models;
using Dapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Features.COAGroups.GetCOAGroupsById
{
    public record GetCOAGroupById(long Id) : IRequest<GetCOAGroupByIdResponse>;

    internal class GetCOAGroupByIdValidator : AbstractValidator<GetCOAGroupById>
    {
        public GetCOAGroupByIdValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0.");
        }
    }
    internal class GetCOAGroupByIdHandler : IRequestHandler<GetCOAGroupById, GetCOAGroupByIdResponse>
    {
        private readonly AccountsDbContext _dbContext;

        public GetCOAGroupByIdHandler(AccountsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetCOAGroupByIdResponse> Handle(GetCOAGroupById request, CancellationToken cancellationToken)
        {
            var connection = _dbContext.Database.GetDbConnection();

            const string sql = @"
                SELECT 
                    g.Id, 
                    g.Name, 
                    g.ParentId, 
                    p.Name AS ParentName, 
                    g.IsActive
                FROM accounts.Groups g
                LEFT JOIN accounts.Groups p ON g.ParentId = p.Id
                WHERE g.Id = @Id";

            var coaGroupDto = await connection.QueryFirstOrDefaultAsync<COAGroupDto>(sql, new { Id = request.Id });

            Guard.Against.Null(coaGroupDto, nameof(coaGroupDto), $"COA Group with Id {request.Id} not found.");

            return new GetCOAGroupByIdResponse(coaGroupDto);
        }
    }

}
