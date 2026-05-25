using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Query;
using Dapper;
using FluentValidation;
using HotelRoomMS.Application.Bookings.Dto;
using HotelRoomMS.Application.Hotels.Dto;
using HotelRoomMS.Application.Hotels.Features.GettingHotels;
using NuGet.Protocol.Plugins;
using System.Globalization;

namespace HotelRoomMS.Application.Bookings.Features.GettingBookingsGrid;

public record GettingBookingGrid : ListQuery<GettingBookingGridResponse>;

public class GettingBookingGridValidator : AbstractValidator<GettingBookingGrid>
{
    public GettingBookingGridValidator()
    {

        RuleFor(x => x.Page)
               .GreaterThanOrEqualTo(0).WithMessage("Page must be >= 0.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(0).WithMessage("PageSize must be >= 0.");
    }
}

internal class GettingBookingGridHandler : IRequestHandler<GettingBookingGrid, GettingBookingGridResponse>
{
    private readonly IDbConnectionCreator _dbConnection;

    public GettingBookingGridHandler(IDbConnectionCreator dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<GettingBookingGridResponse> Handle(GettingBookingGrid request, CancellationToken cancellationToken)
    {
        using var conn = _dbConnection.GetOrCreateConnection();

        var parameters = new DynamicParameters();
        const string countQuery = @"SELECT COUNT(b.id) FROM bookings b JOIN rooms r on r.id = b.roomid /**where**/;";

        const string sqlTemplate = @"WITH _data AS (
                                     SELECT 
                                        b.id,
                                        b.bookingNumber,
                                        b.roomId,
                                        r.RoomNumber,
                                        b.CheckIn,
                                        b.ExpectedCheckOut,
                                        b.CheckOut,
                                        b.RoomPrice,
                                        b.TotalAmount,
                                        b.Discount,
                                        b.TotalPaid,
                                        b.Remarks,
                                        b.IsCancelled,
                                        b.IsActive
                                    FROM bookings b
                                    JOIN rooms r on r.id = b.roomid
                                    /**where**/
                                         )
                                         SELECT
                                            id,
                                            bookingNumber,
                                            roomId,
                                            RoomNumber,
                                            CheckIn,
                                            ExpectedCheckOut,
                                            CheckOut,
                                            RoomPrice,
                                            TotalAmount,
                                            Discount,
                                            TotalPaid,
                                            Remarks,
                                            IsCancelled,
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

                    case "bookingnumber":
                        sqlBuilder.Where($"lower(bookingNumber) like lower('%{filterOption.FieldValue}%')");
                        break;

                    case "checkin":

                        // Expected format: "dd-MM-yyyy to dd-MM-yyyy"
                        var parts = filterOption.FieldValue
                            .Split("to", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                        var fromDate = DateTime.ParseExact(parts[0], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        var toDate = DateTime.ParseExact(parts[1], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        sqlBuilder.Where("CheckIn >= @fromDate AND CheckIn < @toDate");

                        parameters.Add("@fromDate", fromDate);
                        parameters.Add("@toDate", toDate);
                        break;

                    case "checkout":

                        // Expected format: "dd-MM-yyyy to dd-MM-yyyy"
                        var parts2 = filterOption.FieldValue
                            .Split("to", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                        var fromDate2 = DateTime.ParseExact(parts2[0], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        var toDate2 = DateTime.ParseExact(parts2[1], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        sqlBuilder.Where("CheckOut >= @fromDate AND CheckOut < @toDate");

                        parameters.Add("@fromDate", fromDate2);
                        parameters.Add("@toDate", toDate2);
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
                    case "bookingnumber":
                        sqlBuilder.OrderBy($"name");
                        break;
                    case "bookingnumber_desc":
                        sqlBuilder.OrderBy($"name desc");
                        break;
                    default:
                        throw new ArgumentException($"Unsupported sorting: {sort}");
                }
            }
        }
        if (request.Sorts == null || request.Sorts.Count == 0)
        {
            sqlBuilder.OrderBy($"bookingnumber asc");
        }


        var pageData = PagedQueryHelper.GetPageData(request.Page, request.PageSize == int.MaxValue ? 0 : request.PageSize);
        parameters.Add(nameof(PagedQueryHelper.Offset), pageData.Offset);
        parameters.Add(nameof(PagedQueryHelper.Next), pageData.Next);

        var data = await conn.QueryAsync<BookingDto>(template.RawSql, parameters);


        var totalCount = conn.Query<int>(count.RawSql, parameters).First();

        var getData = PagedQueryHelper.CreatePagedResponse(data.ToList(), pageData, totalCount);

        return new GettingBookingGridResponse(getData);
    }
}