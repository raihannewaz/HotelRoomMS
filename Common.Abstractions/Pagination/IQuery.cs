using Common.Abstractions.CQRS;

namespace Common.Abstractions.Pagination;

public interface IQuery<T> : IRequest<T>
    where T : notnull
{
}
