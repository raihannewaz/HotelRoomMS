namespace Common.Abstractions.CQRS
{
    public interface IRequestHandler<TReequest, TResponse> where TReequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TReequest reequest, CancellationToken cancellationToken);
    }

    public interface IRequestHandler<TRequest> : IRequestHandler<TRequest, Unit>
    where TRequest : IRequest<Unit>
    {
    }
}
