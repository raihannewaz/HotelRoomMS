using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Identity.Identity.Users.Dtos;
using Dapper;

namespace Common.Identity.Users.Features.GettingUsers
{
    public class GettingUsers : IRequest<GettingUserResponse>
    {
        public class GettingUserHandler : IRequestHandler<GettingUsers, GettingUserResponse>
        {
            private readonly IDbConnectionCreator _dbConnectionCreator;

            public GettingUserHandler(IDbConnectionCreator dbConnectionCreator)
            {
                _dbConnectionCreator = dbConnectionCreator;
            }

            public async Task<GettingUserResponse> Handle(GettingUsers request, CancellationToken cancellationToken)
            {
                const string sql = @"SELECT  u.[id], u.[first_name] as firstname, u.[last_name] as lastname , u.[user_state] as userstate, 
                                             u.[created_at] as createdat, u.[user_name] as username, u.[email] as email, r.normalized_name as role, 
                                             u.[user_state] as UserStatus 
                                    FROM [users].[asp_net_users] as u 
                                    join [users].[asp_net_user_roles] as anu on u.id = anu.user_id 
                                    join [users].[asp_net_roles] as r on anu.role_id = r.id";

                using (var con = _dbConnectionCreator.GetOrCreateConnection())
                {
                    var users = await con.QueryAsync<UserDto>(sql);
                    Guard.Against.Null(users, nameof(users));

                    return new GettingUserResponse(users.ToList());
                }
            }
        }
    }
}
