using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing
{
    internal interface IThingActivator : IEnumerable<Thing>
    {
        void Register<T>(IServiceProvider service)
            where T : Thing;
        
        void Register<T>(IServiceProvider service, string thing)
            where T : Thing;
        
        void Register<T>(IServiceProvider service, T thing)
            where T : Thing;
        
        Thing CreateInstance(IServiceProvider serviceProvider, string thingName);
    }
}
