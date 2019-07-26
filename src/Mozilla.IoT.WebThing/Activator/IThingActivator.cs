using System;

namespace Mozilla.IoT.WebThing
{
    internal interface IThingActivator
    {
        void RegisterInstance<T>(T thing)
            where T : Thing;
        Thing CreateInstance(IServiceProvider serviceProvider,Type implementationType);
    }
}
