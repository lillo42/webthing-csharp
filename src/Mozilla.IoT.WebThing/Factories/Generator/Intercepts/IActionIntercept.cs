using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator.Intercepts
{
    public interface IActionIntercept : IIntercept
    {
        void Intercept(Thing thing, MethodInfo action, ThingActionAttribute? actionInfo);
    }
}
