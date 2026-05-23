using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Query;
using Dapper;
using FluentValidation;
using HotelRoomMS.Application.Rooms.Dto;

namespace HotelRoomMS.Application.Rooms.Features.GettingRoomsGrid;

public record GettingRoomGrid : ListQuery<GettingRoomGridResponse>;

internal class GettingRoomGridValidator : AbstractValidator<GettingRoomGrid>
{
    public GettingRoomGridValidator()
    {
        RuleFor(x => x.Page)
               .GreaterThanOrEqualTo(0).WithMessage("Page must be >= 0.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(0).WithMessage("PageSize must be >= 0.");
    }
}

internal class GettingRoomGridHandler : IRequestHandler<GettingRoomGrid, GettingRoomGridResponse>
{
    private readonly IDbConnectionCreator _connectionCreator;

    public GettingRoomGridHandler(IDbConnectionCreator connectionCreator)
    {
        _connectionCreator = connectionCreator;
    }

    public async Task<GettingRoomGridResponse> Handle(GettingRoomGrid request, CancellationToken cancellationToken)
    {
        using (var con = _connectionCreator.GetOrCreateConnection())
        {
            var parameters = new DynamicParameters();
            const string countQuery = @"SELECT COUNT(r.id) FROM rooms r 
                                 Join hotels h on r.hotelId = h.id 
                                 Join roomtypes rt on r.roomTypeId = rt.id /**where**/;";

            const string sqlTemplate = @"WITH _data AS (
                                 SELECT r.Id,
                                        r.HotelId,
                                        h.name as HotelName,
                                        r.RoomTypeId,
                                        rt.RoomType,
                                        r.RoomNumber,
                                        r.PricePerDay,
                                        r.IsBooked,
                                        r.IsActive 
                                 FROM rooms r 
                                 Join hotels h on r.hotelId = h.id 
                                 Join roomtypes rt on r.roomTypeId = rt.id 
                                 /**where**/ ) 

                                 SELECT Id,
                                        HotelId,
                                        HotelName,
                                        RoomTypeId,
                                        RoomType,
                                        RoomNumber,
                                        PricePerDay,
                                        IsBooked,
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
                        case "hotelid":
                            sqlBuilder.Where("HotelId = @hotelId");
                            parameters.Add($"@hotelId", filterOption.FieldValue);
                            break;

                        case "roomtypeid":
                            sqlBuilder.Where("RoomTypeId = @roomTypeId");
                            parameters.Add($"@roomTypeId", filterOption.FieldValue);
                            break;

                        case "roomnumber":
                            sqlBuilder.Where($"lower(RoomNumber) like lower('%{filterOption.FieldValue}%')");
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
                        case "roomnumber":
                            sqlBuilder.OrderBy($"name");
                            break;
                        case "roomnumber_desc":
                            sqlBuilder.OrderBy($"name desc");
                            break;
                        default:
                            throw new ArgumentException($"Unsupported sorting: {sort}");
                    }
                }
            }
            if (request.Sorts == null || request.Sorts.Count == 0)
            {
                sqlBuilder.OrderBy($"roomnumber asc");
            }


            var pageData = PagedQueryHelper.GetPageData(request.Page, request.PageSize == int.MaxValue ? 0 : request.PageSize);
            parameters.Add(nameof(PagedQueryHelper.Offset), pageData.Offset);
            parameters.Add(nameof(PagedQueryHelper.Next), pageData.Next);

            var data = await con.QueryAsync<RoomDto>(template.RawSql, parameters);


            var totalCount = con.Query<int>(count.RawSql, parameters).First();

            var getData = PagedQueryHelper.CreatePagedResponse(data.ToList(), pageData, totalCount);

            return new GettingRoomGridResponse(getData);
        }
    }
}
}