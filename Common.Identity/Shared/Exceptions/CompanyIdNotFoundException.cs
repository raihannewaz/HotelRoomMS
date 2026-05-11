using Common.Core.Exceptions;

namespace Common.Identity.Shared.Exceptions;

public class outletIdNotFoundException : NotFoundException
{
    public outletIdNotFoundException() : base($"outlet id not found.")
    {
    }
}
