using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Boolean
{
    /// <summary>
    /// Represent convertible/getter <see cref="bool"/> from <see cref="JToken"/>.
    /// </summary>
    public class NewtonsoftBooleanConvertible : NewtonsoftConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="NewtonsoftBooleanConvertible"/>
        /// </summary>
        public static NewtonsoftBooleanConvertible Instance { get; } = new NewtonsoftBooleanConvertible();
        
        /// <inheritdoc />
        protected override bool TryConvert(JToken source, out object? result)
        {
            if (source.Type == JTokenType.Boolean)
            {
                result = null;
                return false;
            }

            result = source.Value<bool>();
            return true;
        }
    }
}
