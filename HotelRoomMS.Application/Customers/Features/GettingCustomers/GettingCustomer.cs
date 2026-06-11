using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Query;
using Dapper;
using FluentValidation;
using HotelRoomMS.Application.Customers.Dto;
using HotelRoomMS.Application.Hotels.Dto;

namespace HotelRoomMS.Application.Customers.Features.GettingCustomers;

public record GettingCustomer : ListQuery<GettingCustomerResponse>;

public class GettingCustomerValidator : AbstractValidator<GettingCustomer>
{
    public GettingCustomerValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be a positive integer.");
        RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("PageSize must be a positive integer.");
    }

}

public class GettingCustomerHandler : IRequestHandler<GettingCustomer, GettingCustomerResponse>
{
    private readonly IDbConnectionCreator _connectionCreator;

    public GettingCustomerHandler(IDbConnectionCreator connectionCreator)
    {
        _connectionCreator = connectionCreator;
    }

    public async Task<GettingCustomerResponse> Handle(GettingCustomer request, CancellationToken cancellationToken)
    {
        using var con = _connectionCreator.GetOrCreateConnection();

        var parameters = new DynamicParameters();
        const string countQuery = @"SELECT COUNT(d.id) FROM dbo.Customers as d /**where**/;";

        const string sqlTemplate = @"
                                         WITH _data AS (
                                             SELECT 
                                                  d.Id,
                                                  d.FullName,
                                                  d.Phone,
                                                  d.Email,
                                                  d.Address,
                                                  d.NidNumber,
                                                  d.PassportNumber
                                             FROM dbo.Customers as d
                                             /**where**/
                                         )
                                         SELECT
                                               id,
                                               FullName,
                                               Phone,
                                               Email,
                                               Address,
                                               NidNumber,
                                               PassportNumber
                                         FROM _data 
                                         /**orderby**/
                                         OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY;";


        var sqlBuilder = new SqlBuilder();
        var template = sqlBuilder.AddTemplate(sqlTemplate);

        var count = sqlBuilder.AddTemplate(countQuery);


        if (request.Filters != null)
        {
            foreach (var filterOption in request.Filters)
            {
                switch (filterOption.FieldName.ToLower())
                {
                    case "id":
                        sqlBuilder.Where("id = @hotelId");
                        parameters.Add($"@hotelId", filterOption.FieldValue);
                        break;

                    case "name":
                        sqlBuilder.Where($"lower(fullname) like lower('%{filterOption.FieldValue}%')");
                        break;

                    case "passportnumber":
                        sqlBuilder.Where($"lower(passportnumber) like lower('%{filterOption.FieldValue}%')");
                        break;

                    case "nidnumber":
                        sqlBuilder.Where($"lower(nidnumber) like lower('%{filterOption.FieldValue}%')");
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
                        sqlBuilder.OrderBy($"full   ");
                        break;
                    case "name_desc":
                        sqlBuilder.OrderBy($"fullname desc");
                        break;
                    default:
                        throw new ArgumentException($"Unsupported sorting: {sort}");
                }
            }
        }
        if (request.Sorts == null || request.Sorts.Count == 0)
        {
            sqlBuilder.OrderBy($"fullname asc");
        }


        var pageData = PagedQueryHelper.GetPageData(request.Page, request.PageSize == int.MaxValue ? 0 : request.PageSize);
        parameters.Add(nameof(PagedQueryHelper.Offset), pageData.Offset);
        parameters.Add(nameof(PagedQueryHelper.Next), pageData.Next);

        var data = await con.QueryAsync<CustomerDto>(template.RawSql, parameters);


        var totalCount = con.Query<int>(count.RawSql, parameters).First();

        var getData = PagedQueryHelper.CreatePagedResponse(data.ToList(), pageData, totalCount);

        return new GettingCustomerResponse(getData);

    }
}
