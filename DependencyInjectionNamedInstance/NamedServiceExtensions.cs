using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionNamedInstance
{
    public static class NamedServiceExtensions
    {
        public static IServiceCollection AddNamedTransient<T>(this IServiceCollection services, string name, Func<IServiceProvider, T> factory, ServiceLifetime lifetime)
        {
            TypeBuilder tb = GetTypeBuilder(name);
            var type = tb.CreateType();
            object TypeCastFactory(IServiceProvider ctx) => factory(ctx);

            var myType = (ServiceRegistrationWrapper)Activator.CreateInstance(type);
            myType.Target = TypeCastFactory;
            myType.ObjectType = typeof(T);
            myType.Name = name;
            myType.Register();
            services.Add(new ServiceDescriptor(myType.GetType(), ctx => myType.Target.Invoke(ctx), lifetime));


            var factoryRegistration = services.FirstOrDefault(x => x.ServiceType == typeof(NamedServiceFactory<T>));
            if (factoryRegistration == null)
            {
                services.Add(new ServiceDescriptor(typeof(INamedServiceFactory<T>), ctx =>
                {
                    var ctxLocal = ctx;
                    var f1 = new NamedServiceFactory<T>(ctxLocal);
                    return f1;
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