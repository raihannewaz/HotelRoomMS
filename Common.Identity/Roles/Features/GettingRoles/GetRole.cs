using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Identity.Roles.Dtos;
using Common.Identity.Users.Features.GettingUsers;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Identity.Roles.Features.GettingRoles
{
    public class GetRole : IRequest<GettingRolesResponse>
    {
        public class GetRoleHandler : IRequestHandler<GetRole, GettingRolesResponse>
        {
            private readonly IDbConnectionCreator _dbConnection;
            public GetRoleHandler(IDbConnectionCreator dbConnection)
            {
                this._dbConnection = dbConnection;
            }
            public async Task<GettingRolesResponse> Handle(GetRole request, CancellationToken cancellationToken)
            {
                string sql = @"SELECT [id],[name] FROM [users].[asp_net_roles] ";

                using (var con = _dbConnection.GetOrCreateConnection())
                {
                    var roles = await con.QueryAsync<RoleDto>(sql);
                    Guard.Against.Null(roles, nameof(roles));

                    return new GettingRolesResponse(roles.ToList());
                }
            }
        }

    }
}
