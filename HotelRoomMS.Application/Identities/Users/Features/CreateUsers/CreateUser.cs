using Ardalis.GuardClauses;
using BlogAppManage.Application.Identities.Users.Features.CreateUsers.Requests;
using BlogAppManage.Infrastruture.DbContexts;
using Common.Abstractions.CQRS;
using Common.CustomIdentity.Models;
using FluentValidation;

namespace BlogAppManage.Application.Identities.Users.Features.CreateUsers;

public record CreateUser(CreateUserRequest createUser) : IRequest<CreateUserResponse>;



public class CreateUserHandler : IRequestHandler<CreateUser, CreateUserResponse>
{
    private readonly IDbContext _dbContext;


    public class CreateUserValidator : AbstractValidator<CreateUser>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.createUser.UserName).NotEmpty().NotNull().WithMessage("UserName is required.");
            RuleFor(x => x.createUser.FirstName).NotEmpty().NotNull().WithMessage("FirstName is required.");
            RuleFor(x => x.createUser.LastName).NotEmpty().NotNull().WithMessage("LastName is required.");

            RuleFor(x => x.createUser.Password)
                .NotEmpty().WithMessage("Passqord is required.");
        }
    }


    public CreateUserHandler(IDbContext catalogDbContext)  
    {
        _dbContext = Guard.Against.Null(catalogDbContext, nameof(catalogDbContext));
    }

    public async Task<CreateUserResponse> Handle(CreateUser request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        //if (!_securityContextAccessor.Permissions.Contains(FeedbackModulePermissions.MEMBERCREATE, StringComparer.Ordinal))
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);
;


        var user = User.Create(request.createUser.FirstName, request.createUser.LastName,request.createUser.Email,request.createUser.PhoneNumber,request.createUser.UserName, request.createUser.Password);


        await _dbContext.Users.AddAsync(user, cancellationToken: cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var createResponse = UserMapper.QueryResponse(user);

        return new CreateUserResponse(createResponse);

    }
}
