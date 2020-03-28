using System.Text.Json;

namespace Mozilla.IoT.WebThing.Properties
{
    /// <summary>
    /// Get and Set Property of thing
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// Get value of thing
        /// </summary>
        /// <returns>Value of property thing</returns>
        object? GetValue();
        
        /// <summary>
        /// Set value of thing
        /// </summary>
        /// <param name="element">Input value, from buffer</param>
        /// <returns>The <see cref="SetPropertyResult"/>></returns>
        SetPropertyResult SetValue(JsonElement element);
    }
}
