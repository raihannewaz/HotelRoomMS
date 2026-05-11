using Common.Core.Exceptions;

namespace Common.Identity.Identity.Features.VerifyEmail.Exceptions;

public class VerificationTokenIsInvalidException : BadRequestException
{
    public VerificationTokenIsInvalidException(string userId) : base(
        $"verification token is invalid for userId '{userId}'.")
    {
    }
}
