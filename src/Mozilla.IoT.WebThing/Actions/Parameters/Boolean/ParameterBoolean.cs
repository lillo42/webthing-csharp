using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions.Parameters.Boolean
{
    public readonly struct ParameterBoolean : IActionParameter
    {
        public ParameterBoolean(bool isNullable)
        {
            CanBeNull = isNullable;
        }


        public bool CanBeNull { get; }

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
