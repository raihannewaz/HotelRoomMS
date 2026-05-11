namespace Common.Abstractions.Messaging;

public interface IMessage 
{
    Guid MessageId { get; }
    DateTime Created { get; }
}
