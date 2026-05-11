using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Query;
using Common.CustomIdentity.Dto;
using Dapper;
using FluentValidation;

namespace BlogAppManage.Application.Identities.Users.Features.GettingUsers
{
    public record GettingUserGrid : ListQuery<GettingUserGridResponse>;

    public class GetUserGridValidator : AbstractValidator<GettingUserGrid>
    {
        public GetUserGridValidator()
        {

            RuleFor(x => x.Page)
                   .GreaterThanOrEqualTo(0).WithMessage("Page must be >= 0.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(0).WithMessage("PageSize must be >= 0.");
        }
    }

    public class GetUserGridHandler : IRequestHandler<GettingUserGrid, GettingUserGridResponse>
    {
        private readonly IDbConnectionCreator _dbConnection;


        public GetUserGridHandler(IDbConnectionCreator dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<GettingUserGridResponse> Handle(GettingUserGrid request, CancellationToken cancellationToken)
        {
            //if (!_securityContextAccessor.Permissions.Contains(FeedbackModulePermissions.MEMBERVIEW, StringComparer.Ordinal))
            //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

            using (var conn = _dbConnection.GetOrCreateConnection())
            {



                var parameters = new DynamicParameters();
                const string countQuery = @"SELECT COUNT(u.id) FROM dbo.Users as u /**where**/;";

                const string sqlTemplate = @"
                                         WITH _data AS (
                                             SELECT 
                                             	u.id, 
                                             	u.FirstName, 
                                             	u.LastName,
                                             	u.Email,
                                             	u.UserName, 
                                             	u.PhoneNumber
                                             FROM Users u
                                             /**where**/
                                         )
                                         SELECT
                                             	id, 
                                             	FirstName, 
                                             	LastName,
                                             	Email,
                                             	UserName, 
                                             	PhoneNumber
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
                                var (Postname_filterString, shortName_filterValue) = RawSqlFilterExtensions.WhereClauseBuilder("FirstName", filterOption.Comparision, filterOption.FieldValue, "string");
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
                                sqlBuilder.OrderBy($"FirstName");
                                break;
                            case "name_desc":
                                sqlBuilder.OrderBy($"FirstName desc");
                                break;
                            default:
                                throw new ArgumentException($"Unsupported sorting: {sort}");
                        }
                    }
                }
                if (request.Sorts == null || request.Sorts.Count ==0)
                {
                    sqlBuilder.OrderBy($"FirstName asc");
                }


                var pageData = PagedQueryHelper.GetPageData(request.Page, request.PageSize == int.MaxValue ? 0 : request.PageSize);
                parameters.Add(nameof(PagedQueryHelper.Offset), pageData.Offset);
                parameters.Add(nameof(PagedQueryHelper.Next), pageData.Next);

                var data = await conn.QueryAsync<UserDto>(template.RawSql, parameters);


                var totalCount = conn.Query<int>(count.RawSql, parameters).First();

                var getUserData = PagedQueryHelper.CreatePagedResponse(data.ToList(), pageData, totalCount);

                return new GettingUserGridResponse(getUserData);
            }
        }
    }
}
