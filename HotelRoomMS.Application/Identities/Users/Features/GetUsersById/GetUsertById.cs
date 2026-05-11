using Ardalis.GuardClauses;
using BlogAppManage.Application.Posts.Dto;
using BlogAppManage.Application.Tags.Dto;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.CustomIdentity.Dto;
using Dapper;
using FluentValidation;

namespace BlogAppManage.Application.Identities.Users.Features.GetUsersById;

public record GetUsertById(int Id) : IRequest<GetUserByIdResponse>;

internal class GetUserByIdValidator : AbstractValidator<GetUsertById>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class GetUserByIdHandler : IRequestHandler<GetUsertById, GetUserByIdResponse>
{
    private readonly IDbConnectionCreator _dbConnnection;


    public GetUserByIdHandler(IDbConnectionCreator dbConnnection)
    {
        _dbConnnection = dbConnnection;
    }

    public async Task<GetUserByIdResponse> Handle(GetUsertById request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        const string sql = @"
                            SELECT 
                                u.id, 
                                u.FirstName, 
                                u.LastName,
                                u.Email,
                                u.UserName, 
                                u.PhoneNumber
                                FROM Users u
                            WHERE u.id = @userId";

        using var con = _dbConnnection.GetOrCreateConnection();

        var results = await con.QueryFirstOrDefaultAsync<UserDto>(sql, new { userId = request.Id });
        Guard.Against.NotFound($"Post Not Found with ID {request.Id}", results);



        return new GetUserByIdResponse(results);
    }

}


public record GetUserByIdResponse(UserDto User);
