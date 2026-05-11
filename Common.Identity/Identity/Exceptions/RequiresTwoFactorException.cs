

using Common.Core.Exceptions;

namespace Common.Identity.Identity.Exceptions;

public class RequiresTwoFactorException : BadRequestException
{
    public RequiresTwoFactorException(string message) : base(message)
    {
    }
}
