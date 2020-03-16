using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct ActionInfoConvert
    {
        private readonly IReadOnlyDictionary<string, IActionParameter> _actionParameters;

        public ActionInfoConvert(IReadOnlyDictionary<string, IActionParameter> actionParameters)
        {
            _actionParameters = actionParameters ?? throw new ArgumentNullException(nameof(actionParameters));
        }

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
