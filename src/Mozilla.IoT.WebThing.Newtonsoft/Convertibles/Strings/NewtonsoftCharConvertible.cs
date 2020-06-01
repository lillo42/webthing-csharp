using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Strings
{
    /// <summary>
    /// Represent convertible/getter <see cref="char"/> from <see cref="JToken"/>.
    /// </summary>
    public class NewtonsoftCharConvertible : NewtonsoftConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="NewtonsoftCharConvertible"/>
        /// </summary>
        public static NewtonsoftCharConvertible Instance { get; } = new NewtonsoftCharConvertible();

        /// <inheritdoc/>
        protected override bool TryConvert(JToken source, out object? result)
        {
            result = null;
            if (source.Type != JTokenType.String)
            {
                return false;
            }

            var value = source.Value<string>();
            if (value.Length != 1)
            {
                return false;
            }

            result = value[0];
            return true;
        }
    }
}
