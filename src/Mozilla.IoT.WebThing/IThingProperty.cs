using System.Diagnostics.CodeAnalysis;

namespace Mozilla.IoT.WebThing
{
    /// <summary>
    /// Represent the thing property.
    /// </summary>
    public interface IThingProperty
    {
        /// <summary>
        /// Name of origin property
        /// </summary>
        string OriginPropertyName { get; }
        
        /// <summary>
        /// Try get value. it only can get value if property is write-only
        /// </summary>
        /// <param name="value">The result value</param>
        /// <returns>The if could get value</returns>
        bool TryGetValue([MaybeNull] out object? value);
        
        /// <summary>
        /// Try set value
        /// </summary>
        /// <param name="value">Value to be set. This value should be <see cref="System.Text.Json.JsonElement"/></param>
        /// <returns>The <see cref="SetPropertyResult"/>.</returns>
        SetPropertyResult TrySetValue(object value);
    }
}
