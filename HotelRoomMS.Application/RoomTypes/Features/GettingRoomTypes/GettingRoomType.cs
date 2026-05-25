using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Query;
using Dapper;
using FluentValidation;
using HotelRoomMS.Application.Hotels.Dto;
using HotelRoomMS.Application.Hotels.Features.GettingHotels;
using HotelRoomMS.Application.RoomTypes.Dto;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.RoomTypes.Features.GettingRoomTypes;

public record GettingRoomType : ListQuery<GettingRoomTypeResponse>;

public class GettingRoomTypeValidator: AbstractValidator<GettingRoomType>
{
    public GettingRoomTypeValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1.");
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).WithMessage("Page size must be greater than or equal to 1.");
    }
}

public class GettingRoomTypeHandler : IRequestHandler<GettingRoomType, GettingRoomTypeResponse>
{
    private readonly IDbConnectionCreator _dbConnection;
    public GettingRoomTypeHandler(IDbConnectionCreator dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public async Task<GettingRoomTypeResponse> Handle(GettingRoomType request, CancellationToken cancellationToken)
    {
        using (var conn = _dbConnection.GetOrCreateConnection())
        {
            var parameters = new DynamicParameters();
            const string countQuery = @"SELECT COUNT(d.id) FROM dbo.RoomTypes as d /**where**/;";

            const string sqlTemplate = @"
                                         WITH _data AS (
                                             SELECT 
                                                  d.Id,
                                                  d.Name,
                                                  d.BasePrice,
                                                  d.IsActive
                                             FROM dbo.RoomTypes as d
                                             /**where**/
                                         )
                                         SELECT
                                               id,
                                               Name,
                                               BasePrice,
                                               IsActive
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
                            sqlBuilder.Where($"lower(name) like lower('%{filterOption.FieldValue}%')");
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

            var data = await conn.QueryAsync<RoomTypeDto>(template.RawSql, parameters);


            var totalCount = conn.Query<int>(count.RawSql, parameters).First();

            var getData = PagedQueryHelper.CreatePagedResponse(data.ToList(), pageData, totalCount);

            return new GettingRoomTypeResponse(getData);
        }
    }
}
