namespace Common.Abstractions.CQRS
{
    public interface IRequest<TResponse>
    {
    }
    public interface IRequest : IRequest<Unit>
    {
    }
}
