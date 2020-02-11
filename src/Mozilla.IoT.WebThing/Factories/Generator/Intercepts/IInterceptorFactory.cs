namespace Mozilla.IoT.WebThing.Factories.Generator.Intercepts
{
    public interface IInterceptorFactory
    {
        IThingIntercept CreateThingIntercept();
        IPropertyIntercept CreatePropertyIntercept();
        IActionIntercept CreatActionIntercept();
        IEventIntercept CreatEventIntercept();
    }
}
