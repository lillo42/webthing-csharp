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
        /// Retrieve the <see cref="T"/> value.
        /// </summary>
        /// <returns>The <see cref="T"/>.</returns>
        T Deserialize<T>(ReadOnlySpan<byte> values);
        
        /// <summary>
        /// Convert the <see cref="T"/> to <see cref="byte"/>
        /// </summary>
        /// <param name="value">The <see cref="T"/> to be convert.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="byte"/>.</returns>
        byte[] Serialize<T>(T value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, object>> ToEnumerable(object data);
    }
}
