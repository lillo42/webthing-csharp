using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator.Intercepts
{
    public interface IPropertyIntercept
    {
        void Before(Thing thing);
        void Intercept(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? thingPropertyAttribute);
        void After(Thing thing);
    }
}
