using System.Text.Json;

namespace Mozilla.IoT.WebThing.Extensions
{
    public class ThingOption
    {
        public int MaxEventSize { get; set; } = 10;
        public bool IgnoreCase { get; set; } = true;
        public JsonNamingPolicy PropertyNamingPolicy { get; set; } = JsonNamingPolicy.CamelCase;
    }
}
