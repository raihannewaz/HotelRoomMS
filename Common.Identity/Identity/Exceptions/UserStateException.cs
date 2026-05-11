using Common.Core.Exceptions;

namespace Common.Identity.Identity.Exceptions;

public class UserStateException : BadRequestException
{
    public UserStateException(string userId) : base(userId)
    {
    }
}
