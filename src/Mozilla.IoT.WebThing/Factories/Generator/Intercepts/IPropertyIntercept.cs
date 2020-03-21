using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Factories.Generator.Intercepts
{
    /// <summary>
    /// Intercept <see cref="Thing"/> property.
    /// </summary>
    public interface IPropertyIntercept : IIntercept
    {
        /// <summary>
        /// Intercept <see cref="Thing"/> property.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        /// <param name="propertyInfo">The property intercept.</param>
        /// <param name="propertyAttribute">The <see cref="ThingPropertyAttribute"/>.</param>
        void Visit(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? propertyAttribute);
    }
}
