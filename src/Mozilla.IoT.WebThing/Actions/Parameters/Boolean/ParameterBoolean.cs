using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions.Parameters.Boolean
{
    /// <summary>
    /// Represent <see cref="bool"/> action parameter.
    /// </summary>
    public readonly struct ParameterBoolean : IActionParameter
    {
        /// <summary>
        /// Initialize a new instance of <see cref="ParameterBoolean"/>.
        /// </summary>
        /// <param name="isNullable">If action parameter accepts null value.</param>
        public ParameterBoolean(bool isNullable)
        {
            CanBeNull = isNullable;
        }
        
        /// <inheritdoc/>
        public bool CanBeNull { get; }

        /// <inheritdoc/>
        public bool TryGetValue(JsonElement element, out object? value)
        {
            if (CanBeNull && element.ValueKind == JsonValueKind.Null)
            {
                value = null;
                return true;
            }

            switch (element.ValueKind)
            {
                case JsonValueKind.True:
                    value = true;
                    return true;
                case JsonValueKind.False:
                    value = false;
                    return true;
                default:
                    value = null;
                    return false;
            }
        }
    }
}
