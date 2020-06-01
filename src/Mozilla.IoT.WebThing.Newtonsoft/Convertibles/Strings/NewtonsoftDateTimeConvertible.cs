using System;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Strings
{
    /// <summary>
    /// Represent convertible/getter <see cref="DateTime"/> from <see cref="JToken"/>.
    /// </summary>
    public class NewtonsoftDateTimeConvertible : NewtonsoftConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="NewtonsoftDateTimeConvertible"/>
        /// </summary>
        public static NewtonsoftDateTimeConvertible Instance { get; } = new NewtonsoftDateTimeConvertible();

        /// <inheritdoc/>
        protected override bool TryConvert(JToken source, out object? result)
        {
            result = null;
            if (source.Type != JTokenType.Date)
            {
                return false;
            }

            result = source.Value<DateTime>();
            return true;
        }
    }
}
