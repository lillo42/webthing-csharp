using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator.Visitor
{
    internal class ActionVisitor
    {
        public static void Visit(IEnumerable<IActionIntercept> intercepts, Thing thing)
        {
            foreach (var intercept in intercepts)
            {
                intercept.Before(thing);
            }
            
            var thingType = thing.GetType();
            
            var actionsInfo = thingType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !x.IsSpecialName
                            && x.Name != nameof(Equals) && x.Name != nameof(GetType) 
                            && x.Name != nameof(GetHashCode) && x.Name != nameof(ToString));

            foreach (var action in actionsInfo)
            {
                var information = action.GetCustomAttribute<ThingActionAttribute>();
                if (information != null && information.Ignore)
                {
                    continue;
                }

                foreach (var intercept in intercepts)
                {
                    intercept.Intercept(thing, action, information);
                }
            }
            
            foreach (var intercept in intercepts)
            {
                intercept.After(thing);
            }
        }
    }
}
