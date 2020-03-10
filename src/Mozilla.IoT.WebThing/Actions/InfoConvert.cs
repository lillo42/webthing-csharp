using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions
{
    public readonly struct InfoConvert
    {
        private readonly IReadOnlyDictionary<string, IActionParameter> _actionParameters;

        public InfoConvert(IReadOnlyDictionary<string, IActionParameter> actionParameters)
        {
            _actionParameters = actionParameters ?? throw new ArgumentNullException(nameof(actionParameters));
        }

        public bool TryConvert(JsonElement element, out Dictionary<string, object?> input)
        {
            input = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var properties in element.EnumerateObject())
            {
                if (!_actionParameters.TryGetValue(properties.Name, out var @params))
                {
                    return false;
                }

                if (!@params.TryGetValue(properties.Value, out var value))
                {
                    return false;
                }

                input.Add(properties.Name, value);
            }

            foreach (var (property, parameter) in _actionParameters)
            {
                if (!input.ContainsKey(property))
                {
                    if (!parameter.CanBeNull)
                    {
                        return false;
                    }

                    input.Add(property, null);
                }
            }

            return true;
        }
    }
}
