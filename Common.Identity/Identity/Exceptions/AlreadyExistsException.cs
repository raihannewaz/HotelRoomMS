using Common.Core.Exceptions;

namespace Common.Identity.Identity.Exceptions;

public class AlreadyExistsException : ConflictException
{
    public AlreadyExistsException(string message) : base(message)
    {
    }
}
