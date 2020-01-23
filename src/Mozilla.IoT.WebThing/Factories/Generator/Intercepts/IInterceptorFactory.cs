using System;

namespace Mozilla.IoT.WebThing.Factories.Generator.Intercepts
{
    internal interface IInterceptorFactory
    {
        IThingIntercept CreateThingIntercept();
        IPropertyIntercept CreatePropertyIntercept();
        IActionIntercept CreatActionIntercept();
        IEventIntercept CreatEventIntercept();

        Type CreateType();
    }
}
