using System.Linq;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions.Parameters.String
{
    /// <summary>
    /// Represent <see cref="char"/> action parameter.
    /// </summary>
    public class ParameterChar : IActionParameter
    {
        
        private readonly char[]? _enums;

        /// <summary>
        /// Initialize a new instance of <see cref="ParameterChar"/>.
        /// </summary>
        /// <param name="isNullable">If action parameter accepts null value.</param>
        /// <param name="enums">The possible values this action parameter can have.</param>
        public ParameterChar(bool isNullable, char[]? enums)
        {
            CanBeNull = isNullable;
            _enums = enums;
        }
        
        
        /// <inheritdoc/>
        public bool CanBeNull { get; }

        /// <inheritdoc/>
        public bool TryGetValue(JsonElement element, out object? value)
        {
            value = null;
            if (CanBeNull && element.ValueKind == JsonValueKind.Null)
            {
                return true;
            }
            
            if (element.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            var @string = element.GetString();

            if (@string.Length != 1)
            {
                return false;
            }

            var jsonValue = @string[0];
            
            if (_enums != null && _enums.Length > 0 &&  !_enums.Contains(jsonValue))
            {
                return false;
            }

            value = jsonValue;
            return true;
        }
    }
}
