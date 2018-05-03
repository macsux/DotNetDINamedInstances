using System;

namespace DependencyInjectionNamedInstance
{
    public class SomeServiceWithDependency
    {
        public SomeServiceWithDependency(string name, SomeService service)
        {
            Console.WriteLine($"name: {name}, Dependency value: {service.Date.Ticks}");
        }
    }
}