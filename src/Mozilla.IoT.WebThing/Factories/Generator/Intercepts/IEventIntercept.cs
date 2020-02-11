using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator.Intercepts
{
    public interface IEventIntercept : IIntercept
    {
        void Visit(Thing thing, EventInfo @event, ThingEventAttribute? eventInfo);
    }
}
