using Common.Abstractions.Core;
using Common.Abstractions.CQRS;

namespace Common.Core.Command;

public class CommandProcessor : ICommandProcessor
{
    private readonly ISender sender;

    public CommandProcessor(ISender sender)
    {
        this.sender= sender;
    }

    public Task<TResult> SendAsync<TResult>(
        IRequest<TResult> command,
        CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        return sender.Send(command, cancellationToken);
    }
}
