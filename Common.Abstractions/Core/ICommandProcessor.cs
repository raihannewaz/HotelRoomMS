using Common.Abstractions.CQRS;

namespace Common.Abstractions.Core;

public interface ICommandProcessor
{
    Task<TResult> SendAsync<TResult>(IRequest<TResult> command, CancellationToken cancellationToken = default)
        where TResult : notnull;

}
