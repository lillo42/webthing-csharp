using System;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Strings
{
    /// <summary>
    /// Represent convertible/getter <see cref="Guid"/> from <see cref="JToken"/>.
    /// </summary>
    public class NewtonsoftGuidConvertible : NewtonsoftConvertible
    {
        /// <summary>
        /// Static Instance of <see cref="NewtonsoftCharConvertible"/>
        /// </summary>
        public static NewtonsoftGuidConvertible Instance { get; } = new NewtonsoftGuidConvertible();

        /// <inheritdoc/>
        protected override bool TryConvert(JToken source, out object? result)
        {
            result = null;
            if (source.Type != JTokenType.String)
            {
                return false;
            }

            if (!Guid.TryParse(source.Value<string>(), out var value))
            {
                return false;
            }

            result = value;
            return true;
        }
    }
}
