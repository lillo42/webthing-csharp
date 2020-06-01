using System;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Strings
{
    /// <summary>
    /// Represent convertible/getter <see cref="Enum"/> from <see cref="JToken"/>.
    /// </summary>
    public class NewtonsoftEnumConvertible : NewtonsoftConvertible
    {
        private readonly Type _enum;

        /// <summary>
        /// Initialize a new instance of <see cref="NewtonsoftEnumConvertible"/>.
        /// </summary>
        /// <param name="enum">The enum type</param>
        public NewtonsoftEnumConvertible(Type @enum)
        {
            _enum = @enum ?? throw new ArgumentNullException(nameof(@enum));
        }

        /// <inheritdoc/>
        protected override bool TryConvert(JToken source, out object? result)
        {
            result = null;
            if (source.Type != JTokenType.String)
            {
                return false;
            }

            if (!Enum.TryParse(_enum, source.Value<string>(), out var value))
            {
                return false;
            }

            result = value;
            return true;
        }
    }
}
