using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DependencyInjectionNamedInstance
{
    public class Program
    {
        public static void Main()
        {
            var services = new ServiceCollection();
            
            services.AddTransient(typeof(DateTime), ctx => DateTime.Now);
            services.AddTransient<SomeService>();
            services.AddNamedTransient("instance1", ctx => new SomeServiceWithDependency("instance1", ctx.GetService<SomeService>()), ServiceLifetime.Transient);
            services.AddNamedTransient("instance2", ctx => new SomeServiceWithDependency("instance2", ctx.GetService<SomeService>()), ServiceLifetime.Transient);

            var container = services.BuildServiceProvider();
            var f = container.GetService<INamedServiceFactory<SomeServiceWithDependency>>();
            f.Create("instance1");
            Thread.Sleep(100);
            f.Create("instance2");
            Console.ReadLine();
        }


        // Define other methods and classes here
    }
}
