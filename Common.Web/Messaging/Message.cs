using Common.Abstractions.Messaging;

namespace Common.Core.Messaging;

public record Message : IMessage
{
    public Guid MessageId => Guid.NewGuid();
    public DateTime Created { get; } = DateTime.Now;
}
