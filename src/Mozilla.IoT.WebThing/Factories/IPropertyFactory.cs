using System;
using Mozilla.IoT.WebThing.Builders;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <summary>
    /// The factory of <see cref="ThingProperty"/>.
    /// </summary>
    public interface IPropertyFactory
    {
        /// <summary>
        /// Create new instance of <see cref="ThingProperty"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of property.</param>
        /// <param name="jsonSchema">The <see cref="JsonSchema"/>.</param>
        /// <param name="thing"></param>
        /// <param name="setter"></param>
        /// <param name="getter"></param>
        /// <param name="originPropertyName"></param>
        /// <returns>New instance of <see cref="ThingProperty"/>.</returns>
        IThingProperty Create(Type propertyType, JsonSchema jsonSchema, Thing thing, 
            Action<object, object?>? setter, Func<object, object?> getter, string originPropertyName);
    }
}
