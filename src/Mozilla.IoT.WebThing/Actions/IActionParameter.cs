using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions
{
    public interface IActionParameter
    {
        bool CanBeNull { get; }
        bool TryGetValue(JsonElement element, out object? value);
    }
}
