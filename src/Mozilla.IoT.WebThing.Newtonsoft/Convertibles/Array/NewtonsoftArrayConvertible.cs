using System;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Array
{
    /// <summary>
    /// Represent convertible/getter object[] from <see cref="JToken"/>.
    /// </summary>
    public class NewtonsoftArrayConvertible : NewtonsoftConvertible
    {
        private readonly IJsonConvertible _convertible;

        /// <summary>
        /// Initialize a new instance of <see cref="NewtonsoftArrayConvertible"/>.
        /// </summary>
        /// <param name="convertible">The <see cref="IJsonConvertible"/>.</param>
        public NewtonsoftArrayConvertible(IJsonConvertible convertible)
        {
            _convertible = convertible ?? throw new ArgumentNullException(nameof(convertible));
        }

        /// <inheritdoc />
        protected override bool TryConvert(JToken source, out object? result)
        {
            if (source.Type == JTokenType.Array)
            {
                result = null;
                return false;
            }

            var array = source.Value<JArray>();
            var values = new object?[array.Count];

            var i = 0;
            foreach (var token in array)
            {
                if (!_convertible.TryConvert(token, out var value))
                {
                    result = null;
                    return false;
                }
                
                values[i] = value;
                i++;
            }
            
            result = values;
            return true;
        }
    }
}
