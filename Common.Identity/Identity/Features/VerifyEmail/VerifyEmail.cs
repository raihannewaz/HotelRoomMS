using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Core.DateTimeConversions;
using Common.Core.Exceptions;
using Common.Identity.Identity.Features.VerifyEmail.Exceptions;
using Common.Identity.Shared.Data;
using Common.Identity.Shared.Exceptions;
using Common.Identity.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Common.Identity.Identity.Features.VerifyEmail;

public record VerifyEmail(string Email, string Code) : IRequest;

internal class VerifyEmailHandler : IRequestHandler<VerifyEmail>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentityContext _dbContext;
    private readonly ILogger<VerifyEmailHandler> _logger;

    public VerifyEmailHandler(
        UserManager<ApplicationUser> userManager,
        IdentityContext dbContext,
        ILogger<VerifyEmailHandler> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
    }


    public async Task<Unit> Handle(VerifyEmail request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(VerifyEmail));

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UserNotFoundException(request.Email);
        }

        if (user.EmailConfirmed)
        {
            throw new EmailAlreadyVerifiedException(user.Email);
        }

        var emailVerificationCode = await _dbContext.Set<EmailVerificationCode>()
            .Where(x => x.Email == request.Email && x.Code == request.Code && x.UsedAt == null)
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);

        if (emailVerificationCode == null)
        {
            throw new BadRequestException("Either email or code is incorrect.");
        }

        if (DateTimeConversion.UTCToBST() > emailVerificationCode.SentAt.AddMinutes(5))
        {
            throw new BadRequestException("The code is expired.");
        }

        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        emailVerificationCode.UsedAt = DateTimeConversion.UTCToBST();
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Email verified successfully for userId:{UserId}", user.Id);

        return Unit.Value;
    }
}
