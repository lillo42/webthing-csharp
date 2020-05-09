
using System;
using System.Collections.Generic;
using System.Reflection;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Builders
{
    /// <summary>
    /// Create property.
    /// </summary>
    public interface IActionBuilder
    {
        /// <summary>
        /// Set <see cref="Thing"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to be set.</param>
        /// <returns></returns>
        IActionBuilder SetThing(Thing thing);

        /// <summary>
        /// Set <see cref="Thing"/> type.
        /// </summary>
        /// <param name="thingType">The <see cref="Thing"/> typeto be set.</param>
        /// <returns></returns>
        IActionBuilder SetThingType(Type thingType);

        /// <summary>
        /// Set <see cref="ThingOption"/>
        /// </summary>
        /// <param name="option">The <see cref="ThingOption"/> to be set.</param>
        /// <returns></returns>
        IActionBuilder SetThingOption(ThingOption option);
        
        /// <summary>
        /// Add property.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="information">The <see cref="JsonSchema"/> about action.</param>
        IActionBuilder Add(MethodInfo action, ThingActionAttribute? information);
        
        /// <summary>
        /// Add property.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="jsonSchema">The <see cref="JsonSchema"/> about parameter.</param>
        IActionBuilder Add(ParameterInfo parameter, JsonSchema jsonSchema);

        /// <summary>
        /// Build the <see cref="Dictionary{TKey,TValue}"/>
        /// </summary>
        /// <returns>New <see cref="Dictionary{TKey,TValue}"/> of the <see cref="ThingProperty"/></returns>
        Dictionary<string, ActionCollection> Build();
    }
}
