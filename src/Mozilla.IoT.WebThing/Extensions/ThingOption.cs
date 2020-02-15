using System.Text.Json;
using Mozilla.IoT.WebThing.Converts;

namespace Mozilla.IoT.WebThing.Extensions
{
    public class ThingOption
    {
        public int MaxEventSize { get; set; } = 10;
        public bool IgnoreCase { get; set; } = true;
        public bool UseThingAdapterUrl { get; set; }
        public JsonNamingPolicy PropertyNamingPolicy { get; set; } = JsonNamingPolicy.CamelCase;

        private JsonSerializerOptions _options;
        private readonly object _locker = null;
        public JsonSerializerOptions ToJsonSerializerOptions()
        {
            if (_options == null)
            {
                lock (_locker)
                {
                    if (_options == null)
                    {
                        _options = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = PropertyNamingPolicy,
                            DictionaryKeyPolicy = PropertyNamingPolicy,
                            IgnoreNullValues = true,
                            Converters =
                            {
                                new ThingConverter(this)
                            }
                        };
                    }
                }
            }

            return _options;
        }
    }
}
