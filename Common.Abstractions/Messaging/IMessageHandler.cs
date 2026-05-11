using Common.Abstractions.Messaging.Context;

namespace Common.Abstractions.Messaging;

public interface IMessageHandler<in TMessage>
    where TMessage : class, IMessage
{
    Task HandleAsync(IConsumeContext<TMessage> messageContext, CancellationToken cancellationToken = default);
}
