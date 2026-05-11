namespace Common.Abstractions.CQRS
{
    public class Sender(IServiceProvider provider) : ISender
    {
        //public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        //{
        //    var handlerType = typeof(IRequestHandler<,>)
        //        .MakeGenericType(request.GetType(), typeof(TResponse));
        //    dynamic handler = provider.GetService(handlerType);
        //   return handler.Handle((dynamic)request, cancellationToken);
        //}

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var requestType = request.GetType();
            var handlerInterface = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

            var handler = provider.GetService(handlerInterface);
            if (handler == null)
            {
                throw new InvalidOperationException($"Handler for {handlerInterface.Name} not found. Make sure it is registered in DI.");
            }

            var method = handlerInterface.GetMethod("Handle");
            if (method == null)
            {
                throw new InvalidOperationException($"Handle method not found on {handlerInterface.Name}");
            }

            return (Task<TResponse>)method.Invoke(handler, new object[] { request, cancellationToken })!;
        }
    }
}
