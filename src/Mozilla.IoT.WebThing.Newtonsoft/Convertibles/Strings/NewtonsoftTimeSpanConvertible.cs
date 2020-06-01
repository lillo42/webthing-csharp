using System;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Strings
{
    /// <summary>
    /// Represent convertible/getter <see cref="TimeSpan"/> from <see cref="JToken"/>.
    /// </summary>
    public class NewtonsoftTimeSpanConvertible : NewtonsoftConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="NewtonsoftCharConvertible"/>
        /// </summary>
        public static NewtonsoftTimeSpanConvertible Instance { get; } = new NewtonsoftTimeSpanConvertible();

        /// <inheritdoc/>
        protected override bool TryConvert(JToken source, out object? result)
        {
            result = null;
            if (source.Type != JTokenType.String)
            {
                return false;
            }

            if (!TimeSpan.TryParse(source.Value<string>(), out var value))
            {
                return false;
            }

            result = value;
            return true;
        }
    }
}
