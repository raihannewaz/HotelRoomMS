using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Core.Exceptions;
using Common.Core.Query;
using Common.Identity.Shared;
using Common.Identity.Shared.Models;
using Common.Identity.Roles.Dtos;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Roles.Features.GettingRoles;

public record GetRolesGrid : ListQuery<GetRolesResponse>;

public class GetRolesValidator : AbstractValidator<GetRolesGrid>
{
    public GetRolesValidator()
    {


        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page should at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize should at least greater than or equal to 1.");
    }
}

public class GetRolesHandler : IRequestHandler<GetRolesGrid, GetRolesResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDbConnectionCreator _connectionFactory;
    private readonly ISecurityContextAccessor _securityContextAccessor;

    public GetRolesHandler(
        UserManager<ApplicationUser> userManager,
        IDbConnectionCreator connectionFactory,
        ISecurityContextAccessor securityContextAccessor)
    {
        _userManager = userManager;
        _connectionFactory = connectionFactory;
        _securityContextAccessor = securityContextAccessor;
    }

    public async Task<GetRolesResponse> Handle(GetRolesGrid request, CancellationToken cancellationToken)
    {
        //if (Convert.ToInt64(_securityContextAccessor.UserId) > 1 && !_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.ROLEVIEW, StringComparer.Ordinal))
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

        using var conn = _connectionFactory.GetOrCreateConnection();

        //var userId = Convert.ToInt64(_securityContextAccessor.UserId);
        //var tenantId = _securityContextAccessor.TenantId;
        //if (string.IsNullOrEmpty(tenantId))
        //    tenantId = await conn.QueryFirstOrDefaultAsync<string>($"SELECT tenant_id from [users].[asp_net_users] where id = @userId;", new { userId });
        //if (string.IsNullOrEmpty(tenantId))
        //    tenantId = "Main";

        var parameters = new DynamicParameters();
        const string countQuery = "SELECT count([id]) FROM [users].[asp_net_roles] /**where**/";

        const string sqlTemplate = "SELECT [id],[name] FROM [users].[asp_net_roles] " +
            "/**where**/ " +
            "/**orderby**/ " +
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
                    case "name":
                        sqlBuilder.Where($"lower(name) like @name");
                        parameters.Add("@name", $"%{filterOption.FieldValue.ToLower()}%");
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

            if (request.Sorts.Count == 0)
            {
                sqlBuilder.OrderBy($"id desc");
            }
        }
        else
        {
            sqlBuilder.OrderBy($"id desc");
        }

        var pageData = PagedQueryHelper.GetPageData(request.Page, request.PageSize == int.MaxValue ? 0 : request.PageSize);
        parameters.Add(nameof(PagedQueryHelper.Offset), pageData.Offset);
        parameters.Add(nameof(PagedQueryHelper.Next), pageData.Next);

        var data = await conn.QueryAsync<RoleDto>(template.RawSql, parameters);

        var totalCount = conn.Query<int>(count.RawSql, parameters).First();

        var users = PagedQueryHelper.CreatePagedResponse(data.ToList(), pageData, totalCount);


        return new GetRolesResponse(users);
    }
}
