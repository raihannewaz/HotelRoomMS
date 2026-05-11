using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Abstractions.Pagination;
using Common.Core.Query;
using Dapper;
using FluentValidation;
using HotelRoomMS.Application.Hotels.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Hotels.Features.GettingHotels
{
    public record GettingHotel : ListQuery<GettingHotelResponse>;

    public class GettingHotelValidator : AbstractValidator<GettingHotel>
    {
        public GettingHotelValidator()
        {

            RuleFor(x => x.Page)
                   .GreaterThanOrEqualTo(0).WithMessage("Page must be >= 0.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(0).WithMessage("PageSize must be >= 0.");
        }
    }

    public class GettingHotelHandler : IRequestHandler<GettingHotel, GettingHotelResponse>
    {
        private readonly IDbConnectionCreator _dbConnection;


        public GettingHotelHandler(IDbConnectionCreator dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<GettingHotelResponse> Handle(GettingHotel request, CancellationToken cancellationToken)
        {
            using (var conn = _dbConnection.GetOrCreateConnection())
            {
                var parameters = new DynamicParameters();
                const string countQuery = @"SELECT COUNT(d.id) FROM dbo.books as d /**where**/;";

                const string sqlTemplate = @"
                                         WITH _data AS (
                                             SELECT 
                                                  d.Id,
                                                  d.Title,
                                                  d.PublishedMonth,
                                                  d.PublicationName,
                                                  d.AutherName,
                                                  d.CoverImage
                                             FROM dbo.books as d
                                             /**where**/
                                         )
                                         SELECT
                                               id,
                                               Title,
                                               PublishedMonth,
                                               PublicationName,
                                               AutherName,
                                               CoverImage
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
                                var (id_filterString, id_filterValue) = RawSqlFilterExtensions.WhereClauseBuilder(filterOption.FieldName, filterOption.Comparision, filterOption.FieldValue, "long");
                                sqlBuilder.Where(id_filterString);
                                parameters.Add($"@{filterOption.FieldName}", id_filterValue);
                                break;

                            case "name":
                                var (Bookname_filterString, shortName_filterValue) = RawSqlFilterExtensions.WhereClauseBuilder("title", filterOption.Comparision, filterOption.FieldValue, "string");
                                sqlBuilder.Where($"lower(title) like lower('%{filterOption.FieldValue}%')");
                                parameters.Add($"@title", shortName_filterValue);
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
                                sqlBuilder.OrderBy($"title");
                                break;
                            case "name_desc":
                                sqlBuilder.OrderBy($"title desc");
                                break;
                            default:
                                throw new ArgumentException($"Unsupported sorting: {sort}");
                        }
                    }
                }
                if (request.Sorts == null || request.Sorts.Count == 0)
                {
                    sqlBuilder.OrderBy($"title asc");
                }


                var pageData = PagedQueryHelper.GetPageData(request.Page, request.PageSize == int.MaxValue ? 0 : request.PageSize);
                parameters.Add(nameof(PagedQueryHelper.Offset), pageData.Offset);
                parameters.Add(nameof(PagedQueryHelper.Next), pageData.Next);

                var data = await conn.QueryAsync<HotelDto>(template.RawSql, parameters);


                var totalCount = conn.Query<int>(count.RawSql, parameters).First();

                var getData = PagedQueryHelper.CreatePagedResponse(data.ToList(), pageData, totalCount);

                return new GettingHotelResponse(getData);
            }
        }
    }
}
