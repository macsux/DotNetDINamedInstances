using System;

namespace DependencyInjectionNamedInstance
{
    public class SomeService
    {
        public DateTime Date { get; }

        public SomeService(DateTime date)
        {
            Date = date;
        }
    }
}