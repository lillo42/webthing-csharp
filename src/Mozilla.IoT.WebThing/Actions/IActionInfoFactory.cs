using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Actions
{
    /// <summary>
    /// Create new instance of <see cref="ActionInfo"/> based in value of <see cref="Dictionary{TKey,TValue}"/> .
    /// </summary>
    public interface IActionInfoFactory
    {
        /// <summary>
        /// Create new instance of <see cref="ActionInfo"/>.
        /// </summary>
        /// <param name="values">The value of input.</param>
        /// <returns>New instance of <see cref="ActionInfo"/>.</returns>
        ActionInfo CreateActionInfo(Dictionary<string, object?> values);
    }
}
