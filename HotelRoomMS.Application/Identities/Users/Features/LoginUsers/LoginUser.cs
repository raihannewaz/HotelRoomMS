using Ardalis.GuardClauses;
using BlogAppManage.Application.Identities.Users.Features.CreateUsers;
using BlogAppManage.Infrastruture.DbContexts;
using Common.Abstractions.CQRS;
using Common.CustomIdentity.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BlogAppManage.Application.Identities.Users.Features.LoginUsers
{
    public record LoginUser(string UserName, string Password) : IRequest<LoginUserResponse>;


    public class LoginUserHandler : IRequestHandler<LoginUser, LoginUserResponse>
    {
        private readonly IDbContext _dbContext;


        public class LoginUserValidator : AbstractValidator<LoginUser>
        {
            public LoginUserValidator()
            {
                RuleFor(x => x.UserName).NotEmpty().NotNull().WithMessage("UserName is required.");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Passqord is required.");
            }
        }


        public LoginUserHandler(IDbContext catalogDbContext)
        {
            _dbContext = Guard.Against.Null(catalogDbContext, nameof(catalogDbContext));
        }

        public async Task<LoginUserResponse> Handle(LoginUser request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken);

            if (user is null)
            {
                return null;
            }

            return new LoginUserResponse("");


        }
    }

    public record LoginUserResponse(string Token);

}
