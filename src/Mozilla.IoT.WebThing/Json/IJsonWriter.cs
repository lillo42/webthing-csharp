using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing.Json
{
    /// <summary>
    /// Write object to json.
    /// </summary>
    public interface IJsonWriter
    {
        /// <summary>
        /// Write value.
        /// </summary>
        /// <param name="value">The value to be writer</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task WriteAsync<T>(T value);
    }
}
