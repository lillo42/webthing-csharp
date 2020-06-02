using System;
using System.Collections.Generic;

namespace Mozilla.IoT.WebThing.Json
{
    /// <summary>
    /// Read json value from buffer.
    /// </summary>
    public interface IJsonConvert
    {
        /// <summary>
        /// Retrieve the value.
        /// </summary>
        /// <typeparam name="T">The Type to be deserialize.</typeparam>
        /// <param name="values">The <see cref="ReadOnlySpan{T}"/> to be convert.</param>
        /// <returns></returns>
        T Deserialize<T>(ReadOnlySpan<byte> values);
        
        /// <summary>
        /// Convert to <see cref="T:byte[]"/>
        /// </summary>
        /// <param name="value">The value to be convert.</param>
        /// <typeparam name="T">The Type to be serialize.</typeparam>
        /// <returns>The <see cref="T:byte[]"/>.</returns>
        byte[] Serialize<T>(T value);

        /// <summary>
        /// Convert object to <see cref="KeyValuePair{TKey,TValue}"/>
        /// </summary>
        /// <param name="data">The <see cref="object"/> to be converted</param>
        /// <returns>Returns <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/></returns>
        IEnumerable<KeyValuePair<string, object>> ToEnumerable(object data);
    }
}
