using Mozilla.IoT.WebThing.Json.Convertibles;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles
{
    /// <summary>
    /// Represent convertible/getter value from <see cref="JToken"/>.
    /// </summary>
    public abstract class NewtonsoftConvertible : IJsonConvertible
    {
        /// <inheritdoc/>
        public bool TryConvert(object source, out object? result)
        {
            result = null;
            if (source == null)
            {
                return true;
            }

            var token = (JToken)source;
            return token.Type == JTokenType.Null
                   || token.Type == JTokenType.Undefined
                   || TryConvert(source, out result);
        }
        
        /// <summary>
        /// Try convert json value to specific value.
        /// </summary>
        /// <param name="source">The <see cref="JToken"/>.</param>
        /// <param name="result">The result.</param>
        /// <returns>return true if could get value, otherwise return false.</returns>
        protected abstract bool TryConvert(JToken source, out object? result);
    }
}
