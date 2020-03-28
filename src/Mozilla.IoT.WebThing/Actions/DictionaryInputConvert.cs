using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions
{
    /// <summary>
    /// Convert <see cref="JsonElement"/> to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    public readonly struct DictionaryInputConvert
    {
        private readonly IReadOnlyDictionary<string, IActionParameter> _actionParameters;

        /// <summary>
        /// Initialize a new instance of <see cref="DictionaryInputConvert"/>.
        /// </summary>
        /// <param name="actionParameters">The <see cref="Dictionary{TKey,TValue}"/> of <see cref="IActionParameter"/> with all action parameters.</param>
        public DictionaryInputConvert(IReadOnlyDictionary<string, IActionParameter> actionParameters)
        {
            _actionParameters = actionParameters ?? throw new ArgumentNullException(nameof(actionParameters));
        }

        /// <summary>
        /// Try to convert to the <see cref="JsonElement"/> to <see cref="Dictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="element">The <see cref="JsonElement"/> of input values</param>
        /// <param name="input">The <see cref="Dictionary{TKey,TValue}"/>.</param>
        /// <returns>Return true if all parameter is correct, otherwise return false.</returns>
        public bool TryConvert(JsonElement element, [NotNullWhen(true)]out Dictionary<string, object?>? input)
        {
            input = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var properties in element.EnumerateObject())
            {
                if (!_actionParameters.TryGetValue(properties.Name, out var @params))
                {
                    input = null;
                    return false;
                }

                if (!@params.TryGetValue(properties.Value, out var value))
                {
                    input = null;
                    return false;
                }
                
                if (!input.TryAdd(properties.Name, value))
                {
                    input = null;
                    return false;
                }
            }

            foreach (var (property, parameter) in _actionParameters)
            {
                if (!input.ContainsKey(property))
                {
                    if (!parameter.CanBeNull)
                    {
                        input = null;
                        return false;
                    }

                    input.Add(property, null);
                }
            }

            return true;
        }
    }
}
