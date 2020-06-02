using System.Text.Json;

namespace Mozilla.IoT.WebThing.Extensions
{
    /// <summary>
    /// The thing option.
    /// </summary>
    public class ThingOption
    {
        /// <summary>
        /// Max size of <see cref="Events.EventCollection"/>.
        /// The default value is 10.
        /// </summary>
        public int MaxEventSize { get; set; } = 10;

        /// <summary>
        /// If should ignore case to deserialize.
        /// The default value is true.
        /// </summary>
        public bool IgnoreCase { get; set; } = true;


        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreNullValues { get; set; } = true;

        /// <summary>
        /// If when serialize thing should serialize for use thing adapter.
        /// The default value is true.
        /// </summary>
        public bool UseThingAdapterUrl { get; set; } = true;
        
        /// <summary>
        /// Specifies the policy used to convert a property's name on an object to another format, such as camel-casing.
        /// The resulting property name is expected to match the JSON payload during deserialization, and
        /// will be used when writing the property name during serialization.
        /// </summary>
        public JsonNamingPolicy PropertyNamingPolicy { get; set; } = JsonNamingPolicy.CamelCase;
        
        internal bool WriteIndented { get; set; }
    }
}
