using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator.Intercepts
{
    public interface IPropertyIntercept
    {
        void BeforeVisit(Thing thing);
        void Intercept(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? thingPropertyAttribute);
        void AfterVisit(Thing thing);
    }
}
