using System;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Strings
{
    /// <summary>
    /// Represent convertible/getter <see cref="String"/> from <see cref="JToken"/>.
    /// </summary>
    public class NewtonsoftStringConvertible : NewtonsoftConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="NewtonsoftCharConvertible"/>
        /// </summary>
        public static NewtonsoftStringConvertible Instance { get; } = new NewtonsoftStringConvertible();

        /// <inheritdoc/>
        protected override bool TryConvert(JToken source, out object? result)
        {
            result = null;
            if (source.Type != JTokenType.String)
            {
                return false;
            }

            result = source.Value<string>();
            return true;
        }
    }
}
