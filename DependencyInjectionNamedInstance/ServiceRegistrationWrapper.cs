using System;
using System.Collections.Generic;

namespace DependencyInjectionNamedInstance
{
    public class ServiceRegistrationWrapper
    {
        public static Dictionary<Tuple<string, Type>, Type> ImplementationToRegistrationMap = new Dictionary<Tuple<string, Type>, Type>();
        public Func<IServiceProvider, object> Target { get; set; }

    }
}