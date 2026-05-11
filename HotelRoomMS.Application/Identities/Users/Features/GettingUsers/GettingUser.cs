using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.CustomIdentity.Dto;
using Dapper;

namespace BlogAppManage.Application.Identities.Users.Features.GettingUsers
{
    public record GettingUser() : IRequest<GettingUserResponse>;

    public class GettingUserHandler : IRequestHandler<GettingUser, GettingUserResponse>
    {
        private readonly IDbConnectionCreator _dbConnectionCreator;

        public GettingUserHandler(IDbConnectionCreator dbConnectionCreator)
        {
            _dbConnectionCreator = dbConnectionCreator;
        }

        public async Task<GettingUserResponse> Handle(GettingUser request, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT 
                                	u.id, 
                                	u.FirstName, 
                                	u.LastName,
                                	u.Email,
                                	u.UserName, 
                                	u.PhoneNumber
                                FROM Users u ";

            using (var con = _dbConnectionCreator.GetOrCreateConnection())
            {
                var data = await con.QueryAsync<UserDto>(sql);

                return new GettingUserResponse(data.ToList());
            }
        }
    }
}
