using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Number
{
    /// <summary>
    /// Represent convertible/getter <see cref="decimal"/> from <see cref="JToken"/>.
    /// </summary>
    public class NewtonsoftNumberConvertible : NewtonsoftConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="NewtonsoftNumberConvertible"/>
        /// </summary>
        public static NewtonsoftNumberConvertible Instance { get; } = new NewtonsoftNumberConvertible();

        
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
