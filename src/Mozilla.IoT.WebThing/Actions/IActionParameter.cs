using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions
{
    /// <summary>
    /// Represent parameter of action.
    /// </summary>
    public interface IActionParameter
    {
        /// <summary>
        /// If this parameter accepts null values.
        /// </summary>
        bool CanBeNull { get; }
        
        /// <summary>
        /// Try get value from <see cref="JsonElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="JsonElement"/>.</param>
        /// <param name="value">The value inside of <see cref="JsonElement"/>.</param>
        /// <returns>return true if <see cref="JsonElement"/> value match with rules, otherwise return false.</returns>
        bool TryGetValue(JsonElement element, out object? value);
    }
}
