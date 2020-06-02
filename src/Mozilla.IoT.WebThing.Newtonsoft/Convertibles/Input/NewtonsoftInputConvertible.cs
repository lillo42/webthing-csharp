using System;
using System.Collections.Generic;
using Mozilla.IoT.WebThing.Json.Convertibles;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft.Convertibles.Input
{
    /// <summary>
    /// Represent convertible/getter to input from <see cref="JToken"/>.
    /// </summary>
    public class NewtonsoftInputConvertible : NewtonsoftConvertible
    {
        private readonly Dictionary<string, IJsonConvertible> _convertibles;
        
        /// <summary>
        /// Initialize a new instance of <see cref="NewtonsoftInputConvertible"/>.
        /// </summary>
        /// <param name="convertibles">The convertibles.</param>
        public NewtonsoftInputConvertible(Dictionary<string, IJsonConvertible> convertibles)
        {
            _convertibles = convertibles;
        }

        /// <inheritdoc/>
        protected override bool TryConvert(JToken source, out object? result)
        {
            result = null;
            var input = source["input"];
            if (input == null)
            {
                return false;
            }
            
            var dict = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase);

            if (input.Type != JTokenType.Object)
            {
                return false;
            }

            foreach (var (name, jsonValue) in input.Value<JObject>())
            {
                if (!_convertibles.TryGetValue(name, out var convertible))
                {
                    return false;
                }

                if (!convertible.TryConvert(jsonValue!, out var value))
                {
                    return false;
                }
                
                dict.Add(name, value);
            }

            result = dict;
            return true;
        }
    }
}
