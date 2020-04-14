using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing.Json
{
    /// <summary>
    /// Read json value from buffer.
    /// </summary>
    public interface IJsonReader
    {
        /// <summary>
        /// Retrieve the <see cref="Dictionary{TKey,TValue}"/> value.
        /// </summary>
        /// <returns>The  <see cref="Dictionary{TKey,TValue}"/>.</returns>
        Task<Dictionary<string, object?>> GetValuesAsync();
    }
}
