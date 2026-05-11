using Ardalis.GuardClauses;
using BlogAppManage.Application.Identities.Users.Features.UpdateUsers.Requests;
using BlogAppManage.Infrastruture.DbContexts;
using Common.Abstractions.CQRS;
using FluentValidation;

namespace BlogAppManage.Application.Identities.Users.Features.UpdateUsers
{
    public record UpdateUser(UpdateUserRequest updateUser) : IRequest<UpdateUserResponse>
    {
    }

    public class UpdateUserValidator : AbstractValidator<UpdateUser>
    {
        public UpdateUserValidator()
        {

            RuleFor(x => x.updateUser.Id).NotEmpty().NotNull();
            RuleFor(x => x.updateUser.FirstName).NotEmpty().NotNull().WithMessage("Title is required.");

            RuleFor(x => x.updateUser.LastName)
                .NotEmpty().WithMessage("Content is required.");
        }
    }

    public class UpdateUsernHandler : IRequestHandler<UpdateUser, UpdateUserResponse>
    {
        private readonly IDbContext _dbContext;


        public UpdateUsernHandler(
            IDbContext dbContext)
        {

            _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));

        }

        public async Task<UpdateUserResponse> Handle(UpdateUser request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            //if (!_securityContextAccessor.Permissions.Contains(FeedbackModulePermissions.MEMBEREDIT, StringComparer.Ordinal))
            //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

            var user = await _dbContext.FindUserAsync(request.updateUser.Id);
            Guard.Against.NotFound($"Post Not Found {request.updateUser.Id}", user);

            user.Update(request.updateUser.FirstName, request.updateUser.LastName, request.updateUser.PhoneNumber);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var updateResponse = UserMapper.QueryResponse(user);

            return new UpdateUserResponse(updateResponse);
        }
    }
}
