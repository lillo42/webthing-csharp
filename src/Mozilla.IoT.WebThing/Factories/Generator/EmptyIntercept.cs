using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    /// <summary>
    /// Empty Intercept.
    /// </summary>
    public class EmptyIntercept : IThingIntercept, IActionIntercept, IEventIntercept, IPropertyIntercept
    {
        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        public void Before(Thing thing)
        {
            
        }
        
        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        public void After(Thing thing)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="event"></param>
        /// <param name="eventInfo"></param>
        public void Visit(Thing thing, EventInfo @event, ThingEventAttribute? eventInfo)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="action"></param>
        /// <param name="actionInformation"></param>
        public void Intercept(Thing thing, MethodInfo action, ThingActionAttribute? actionInformation)
        {
            
        }
        
        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="propertyAttribute"></param>
        public void Visit(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? propertyAttribute)
        {
            
        }
    }
}
