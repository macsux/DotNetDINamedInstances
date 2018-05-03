using System;
using System.Collections.Generic;

namespace DependencyInjectionNamedInstance
{
    public class ServiceRegistrationWrapper
    {
        public static Dictionary<Tuple<string, Type>, Type> ImplementationToRegistrationMap = new Dictionary<Tuple<string, Type>, Type>();
        public Func<IServiceProvider, object> Target { get; set; }
        public Type ObjectType { get; set; }
        public string Name { get; set; }

        public void Register()
        {
            ImplementationToRegistrationMap[Tuple.Create(Name, ObjectType)] = this.GetType();
        }
    }
}