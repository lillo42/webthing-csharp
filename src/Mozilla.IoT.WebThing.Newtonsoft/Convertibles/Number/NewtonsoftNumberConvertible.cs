using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Number
{
    /// <summary>
    /// Represent convertible/getter <see cref="bool"/> from <see cref="JToken"/>.
    /// </summary>
    public class NewtonsoftNumberConvertible : NewtonsoftConvertible
    {
        /// <inheritdoc/>
        protected override bool TryConvert(JToken source, out object? result)
        {
            if (source.Type == JTokenType.Integer || source.Type == JTokenType.Float)
            {
                result = source.Value<decimal>();
                return true;
            }

            result = null;
            return false;
        }
    }
}
