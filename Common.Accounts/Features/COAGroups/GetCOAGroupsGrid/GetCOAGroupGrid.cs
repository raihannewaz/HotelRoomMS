using Common.Abstractions.CQRS;
using Common.Accounts.Data;
using Common.Accounts.DTOs;
using Common.Core.Query;
using Dapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Common.Accounts.Features.COAGroups.GetCOAGroupsGrid;

public record GetCOAGroupGrid : ListQuery<GetCOAGroupGridResponse>;

public class GetCOAGroupGridValidator : AbstractValidator<GetCOAGroupGrid>
{
    public GetCOAGroupGridValidator()
    {
        RuleFor(x => x.Page)
               .GreaterThanOrEqualTo(0).WithMessage("Page must be >= 0.");
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(0).WithMessage("PageSize must be >= 0.");
    }
}


public class GetCOAGroupGridHandler : IRequestHandler<GetCOAGroupGrid, GetCOAGroupGridResponse>
{
    private readonly AccountsDbContext _dbContext;
    public GetCOAGroupGridHandler(AccountsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetCOAGroupGridResponse> Handle(GetCOAGroupGrid request, CancellationToken cancellationToken)
    {
        var connection = _dbContext.Database.GetDbConnection();

        const string countQuery = @"SELECT COUNT(d.id) FROM accounts.Groups as d /**where**/;";

        const string sql = @" WITH _data AS (
                                           SELECT 
                                               g.Id, 
                                               g.Name, 
                                               g.ParentId, 
                                               p.Name AS ParentName, 
                                               g.IsActive
                                           FROM accounts.Groups g
                                           LEFT JOIN accounts.Groups p ON g.ParentId = p.Id
                                           /**where**/
                                           )
                                           SELECT
                                                 Id,
                                                 Name,
                                                 ParentId,
                                                 ParentName,
                                                 IsActive
                                           FROM _data 
                                           /**orderby**/
                                           OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY;";


        var parameters = new DynamicParameters();

        var sqlBuilder = new SqlBuilder();
        var template = sqlBuilder.AddTemplate(sql);
        var count = sqlBuilder.AddTemplate(countQuery);


        if (request.Filters != null)
        {
            foreach (var filterOption in request.Filters)
            {
                switch (filterOption.FieldName.ToLower())
                {
                    case "id":
                        var (id_filterString, id_filterValue) = RawSqlFilterExtensions.WhereClauseBuilder(filterOption.FieldName, filterOption.Comparision, filterOption.FieldValue, "long");
                        sqlBuilder.Where(id_filterString);
                        parameters.Add($"@{filterOption.FieldName}", id_filterValue);
                        break;

                    case "name":

                        sqlBuilder.Where($"lower(name) like lower('%@name%')");
                        parameters.Add($"@name", filterOption.FieldValue);
                        break;

                    case "parentname":
                        sqlBuilder.Where($"lower(parentname) like lower('%@parentname%')");
                        parameters.Add($"@parentname", filterOption.FieldValue);
                        break;


                    default:
                        throw new ArgumentException($"Unsupported filter field: {filterOption.FieldName}");
                }
            }
        }

        if (request.Sorts != null)
        {
            foreach (var sort in request.Sorts)
            {
                switch (sort.ToLower())
                {
                    case "name":
                        sqlBuilder.OrderBy($"name");
                        break;
                    case "name_desc":
                        sqlBuilder.OrderBy($"name desc");
                        break;
                    default:
                        throw new ArgumentException($"Unsupported sorting: {sort}");
                }
            }
        }
        if (request.Sorts == null || request.Sorts.Count == 0)
        {
            sqlBuilder.OrderBy($"name asc");
        }


        var pageData = PagedQueryHelper.GetPageData(request.Page, request.PageSize == int.MaxValue ? 0 : request.PageSize);
        parameters.Add(nameof(PagedQueryHelper.Offset), pageData.Offset);
        parameters.Add(nameof(PagedQueryHelper.Next), pageData.Next);

        var data = await connection.QueryAsync<COAGroupDto>(template.RawSql, parameters);


        var totalCount = connection.Query<int>(count.RawSql, parameters).First();

        var getFinalData = PagedQueryHelper.CreatePagedResponse(data.ToList(), pageData, totalCount);


        return new GetCOAGroupGridResponse(getFinalData);
    }
}


