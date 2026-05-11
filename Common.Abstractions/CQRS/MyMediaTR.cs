using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Common.Abstractions.CQRS
{
    public static class MyMediaTR
    {
        public static IServiceCollection AddMyMediaTR(this IServiceCollection services, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            services.AddScoped<ISender, Sender>();

             var handlerInterfaceType= typeof(IRequestHandler<,>);

            var handlerTypes = assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                .Select(i => new {Interface = i, Implementation = t}));

            foreach (var handler in handlerTypes)
            {
                services.AddScoped(handler.Interface, handler.Implementation);  
            }

            return services;
        }
    }
}
