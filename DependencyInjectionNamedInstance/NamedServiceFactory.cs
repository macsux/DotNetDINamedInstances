using System;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionNamedInstance
{
    public class NamedServiceFactory<T> : INamedServiceFactory<T>
    {
        private readonly IServiceProvider _serviceProvider;
        public NamedServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public T Create(string name)
        {
            if (!ServiceRegistrationWrapper.ImplementationToRegistrationMap.TryGetValue(Tuple.Create(name, typeof(T)), out var markerType))
                throw new InvalidOperationException($"No service for type {typeof(T)} named '{name}' has been registered");

            return (T)_serviceProvider.GetRequiredService(markerType);

        }
    }

    public interface INamedServiceFactory<T>
    {
        T Create(string name);
    }

}