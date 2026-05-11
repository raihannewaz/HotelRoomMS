using Common.Core.Exceptions;

namespace Common.Identity.Shared.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string emailOrUserName) : base(
        $"User with email or username: '{emailOrUserName}' not found.")
    {
    }

    public UserNotFoundException(long id) : base($"User with id: '{id}' not found.")
    {
    }
}
