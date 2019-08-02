using System;

namespace Mozilla.IoT.WebThing
{
    internal interface IThingActivator
    {
        void Register<T>(T thing, IServiceProvider service)
            where T : Thing;
        void Register<T>(string thing)
            where T : Thing;
        
        Thing CreateInstance(IServiceProvider serviceProvider, string thingName);
    }
}
