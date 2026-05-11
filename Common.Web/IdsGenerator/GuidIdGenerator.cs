using Common.Abstractions.Core;

namespace Common.Core.IdsGenerator;

public class GuidIdGenerator : IIdGenerator<Guid>
{
    public Guid New()
    {
        return Guid.NewGuid();
    }
}
