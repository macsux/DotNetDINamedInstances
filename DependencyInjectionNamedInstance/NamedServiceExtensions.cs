using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionNamedInstance
{
    public static class NamedServiceExtensions
    {
        // Relies on creating new types at runtime that map named registrations inside container to named instances
        // Factory class knows how to look things up using these dynamic types from service container
        public static IServiceCollection AddNamedTransient<T>(this IServiceCollection services, string name, Func<IServiceProvider, T> factory, ServiceLifetime lifetime)
        {
            TypeBuilder tb = GetTypeBuilder(name);
            var serviceType = tb.CreateType();
            var serviceRegistrationWrapper = new ServiceRegistrationWrapper() {Target = ctx => factory(ctx)};
            ServiceRegistrationWrapper.ImplementationToRegistrationMap[Tuple.Create(name, typeof(T))] = serviceType;

            services.Add(new ServiceDescriptor(serviceType, ctx => serviceRegistrationWrapper.Target.Invoke(ctx), lifetime));

            var factoryRegistration = services.FirstOrDefault(x => x.ServiceType == typeof(NamedServiceFactory<T>));
            if (factoryRegistration == null)
            {
                services.Add(new ServiceDescriptor(typeof(INamedServiceFactory<T>), ctx =>
                {
                    var ctxLocal = ctx;
                    return new NamedServiceFactory<T>(ctxLocal);
                }, lifetime)); // potentially wanna always go to transient here, need to discuss
            }
            return services;
        }
        public static IServiceCollection AddNamedTransient(this IServiceCollection services, string name, object target)
        {
            return AddNamedTransient(services, name, ctx => target, ServiceLifetime.Singleton);
        }

        private static TypeBuilder GetTypeBuilder(string name)
        {
            var typeSignature = name;
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            //AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                null);
            tb.SetParent(typeof(ServiceRegistrationWrapper));
            return tb;
        }
    }
}