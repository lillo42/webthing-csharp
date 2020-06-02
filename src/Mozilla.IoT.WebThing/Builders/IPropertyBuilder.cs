using System.Collections.Generic;
using System.Reflection;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Builders
{
    /// <summary>
    /// Create property.
    /// </summary>
    public interface IPropertyBuilder
    {
        /// <summary>
        /// Set <see cref="Thing"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to be set.</param>
        /// <returns></returns>
        IPropertyBuilder SetThing(Thing thing);

        /// <summary>
        /// Set <see cref="ThingOption"/>
        /// </summary>
        /// <param name="option">The <see cref="ThingOption"/> to be set.</param>
        /// <returns></returns>
        IPropertyBuilder SetThingOption(ThingOption option);
        
        /// <summary>
        /// Add property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="jsonSchema">The <see cref="JsonSchema"/> about property</param>
        void Add(PropertyInfo property, JsonSchema jsonSchema);

        /// <summary>
        /// Build the <see cref="Dictionary{TKey,TValue}"/>
        /// </summary>
        /// <returns>New <see cref="Dictionary{TKey,TValue}"/> of the <see cref="ThingProperty"/></returns>
        Dictionary<string, IThingProperty> Build();
    }
}
