using System;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Strings
{
    /// <summary>
    /// Represent convertible/getter <see cref="DateTimeOffset"/> from <see cref="JToken"/>.
    /// </summary>
    public class NewtonsoftDateTimeOffsetConvertible : NewtonsoftConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="NewtonsoftDateTimeOffsetConvertible"/>
        /// </summary>
        public static NewtonsoftDateTimeOffsetConvertible Instance { get; } = new NewtonsoftDateTimeOffsetConvertible();

        /// <inheritdoc/>
        protected override bool TryConvert(JToken source, out object? result)
        {
            result = null;
            if (source.Type != JTokenType.Date)
            {
                return false;
            }

            result = source.Value<DateTimeOffset>();
            return true;
        }
    }
}
