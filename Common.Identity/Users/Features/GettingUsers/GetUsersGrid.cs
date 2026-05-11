using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Exceptions;
using Common.Core.Query;
using Common.Identity.Identity.Users.Dtos;
using Common.Identity.Shared;
using Dapper;
using FluentValidation;

namespace Common.Identity.Users.Features.GettingUsers;

public record GetUsersGrid : ListQuery<GetUsersResponse>;

public class GetUsersValidator : AbstractValidator<GetUsersGrid>
{
    public GetUsersValidator()
    {


        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page should at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize should at least greater than or equal to 1.");
    }
}

public class GetUsersHandler : IRequestHandler<GetUsersGrid, GetUsersResponse>
{

    private readonly IDbConnectionCreator _connectionFactory;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public GetUsersHandler(
        IDbConnectionCreator connectionFactory,
        ISecurityContextAccessor securityContextAccessor)
    {

        _connectionFactory = connectionFactory;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<GetUsersResponse> Handle(GetUsersGrid request, CancellationToken cancellationToken)
    {
        //if (!_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.USERVIEW, StringComparer.Ordinal))
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

        using var conn = _connectionFactory.GetOrCreateConnection();

        var userId = Convert.ToInt64(_securityContextAccessor.UserId);
        var tenantId = _securityContextAccessor.TenantId;
        //if (string.IsNullOrEmpty(tenantId))
        //    tenantId = await conn.QueryFirstOrDefaultAsync<string>($"SELECT tenant_id from [users].[asp_net_users] where id = @userId;", new { userId });
        //if (string.IsNullOrEmpty(tenantId))
        //    tenantId = "Main";

        var parameters = new DynamicParameters();
        const string countQuery = "select count(dt.[id]) from " +
            "(SELECT distinct u.[id], u.[first_name] as firstname, u.[last_name] as lastname , u.[last_logged_in_at] as lastloggedinat,  " +
            "u.[user_state] as userstate , u.[created_at] as createdat, u.[user_name] as username, u.[email] as email,  " +
            "r.normalized_name as role, u.[user_state] as UserStatus   " +
            "FROM [users].[asp_net_users] as u  " +
            "join [users].[asp_net_user_roles] as anu on u.id = anu.user_id  " +
            "join [users].[asp_net_roles] as r on anu.role_id = r.id) as dt /**where**/";

        const string sqlTemplate = ";with _data as ( select dt.[id],dt.firstname, dt.lastname ,dt.lastloggedinat,  dt.userstate ," +
            "dt.createdat, dt.username, dt.email, dt.role, dt.UserStatus   " +
            "from (" +
            "SELECT distinct u.[id], u.[first_name] as firstname, u.[last_name] as lastname , u.[last_logged_in_at] as lastloggedinat,   " +
            "u.[user_state] as userstate , u.[created_at] as createdat, u.[user_name] as username, u.[email] as email,   " +
            "r.normalized_name as role,u.[user_state] as UserStatus    " +
            "FROM [users].[asp_net_users] as u " +
            "join [users].[asp_net_user_roles] as anu on u.id = anu.user_id " +
            "join [users].[asp_net_roles] as r on anu.role_id = r.id) as dt /**where**/) " +
            "select [id],firstname,lastname ,lastloggedinat,  userstate ,createdat, username, email, role , UserStatus  " +
            "from _data  /**orderby**/  " +
            "OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY;";

        var sqlBuilder = new SqlBuilder();
        var template = sqlBuilder.AddTemplate(sqlTemplate);

        var count = sqlBuilder.AddTemplate(countQuery);

        if (request.Filters != null)
        {
            foreach (var filterOption in request.Filters)
            {
                switch (filterOption.FieldName.ToLower())
                {
                    case "firstname":
                        sqlBuilder.Where($"lower(firstname) like @firstname");
                        parameters.Add("@firstname", $"%{filterOption.FieldValue.ToLower()}%");
                        break;
                    case "lastname":
                        sqlBuilder.Where($"lower(lastname) like @lastname");
                        parameters.Add("@lastname", $"%{filterOption.FieldValue.ToLower()}%");
                        break;
                    case "email":
                        sqlBuilder.Where($"lower(email) like @email");
                        parameters.Add("@email", $"%{filterOption.FieldValue.ToLower()}%");
                        break;
                    case "username":
                        sqlBuilder.Where($"lower(username) like @username");
                        parameters.Add("@username", $"%{filterOption.FieldValue.ToLower()}%");
                        break;
                    case "role":
                        sqlBuilder.Where($"lower(role) like @role");
                        parameters.Add("@role", $"%{filterOption.FieldValue.ToLower()}%");
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
                    case "firstname":
                        sqlBuilder.OrderBy($"firstname");
                        break;
                    case "firstname_desc":
                        sqlBuilder.OrderBy($"firstname desc");
                        break;
                    case "lastname":
                        sqlBuilder.OrderBy($"lastname");
                        break;
                    case "lastname_desc":
                        sqlBuilder.OrderBy($"lastname desc");
                        break;
                    case "email":
                        sqlBuilder.OrderBy($"email");
                        break;
                    case "email_desc":
                        sqlBuilder.OrderBy($"email desc");
                        break;
                    case "username":
                        sqlBuilder.OrderBy($"username");
                        break;
                    case "username_desc":
                        sqlBuilder.OrderBy($"username desc");
                        break;
                    case "date":
                        sqlBuilder.OrderBy($"createdat");
                        break;
                    case "date_desc":
                        sqlBuilder.OrderBy($"createdat desc");
                        break;
                    default:
                        throw new ArgumentException($"Unsupported sorting: {sort}");
                }
            }

            if (request.Sorts.Count == 0)
            {
                sqlBuilder.OrderBy($"createdat desc");
            }
        }
        else
        {
            sqlBuilder.OrderBy($"createdat desc");
        }

        sqlBuilder.Where("UserStatus <> 'Deleted'");



        var pageData = PagedQueryHelper.GetPageData(request.Page, request.PageSize == int.MaxValue ? 0 : request.PageSize);
        parameters.Add(nameof(PagedQueryHelper.Offset), pageData.Offset);
        parameters.Add(nameof(PagedQueryHelper.Next), pageData.Next);

        var data = await conn.QueryAsync<UserDto>(template.RawSql, parameters);

        var totalCount = conn.Query<int>(count.RawSql, parameters).First();

        var users = PagedQueryHelper.CreatePagedResponse(data.ToList(), pageData, totalCount);

        return new GetUsersResponse(users);
    }
}
