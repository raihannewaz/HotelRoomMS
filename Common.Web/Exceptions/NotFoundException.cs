using System.Net;

namespace Common.Core.Exceptions;

public class NotFoundException : CustomException
{
    public NotFoundException(string message) : base(message)
    {
        StatusCode = HttpStatusCode.NotFound;
    }
}
