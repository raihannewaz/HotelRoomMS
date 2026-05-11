using Common.Core.Exceptions;

namespace Common.Identity.Identity.Exceptions;

public class UserCannotDeleteException : ApiException
{
    public UserCannotDeleteException(string message) : base(message)
    {
    }
}
