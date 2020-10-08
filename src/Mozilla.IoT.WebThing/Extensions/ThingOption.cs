using System;
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

        /// <summary>
        /// The server name.
        /// </summary>
        public string ServerName { get; set; } = string.Empty;

        /// <summary>
        /// If use mDNS
        /// </summary>
        public bool RegistermDNS { get; set; } = true;
        
        /// <summary>
        /// The delay before execute mDNS advertise. It's necessary to the application know which port was bind by ASP.NET Core
        /// The default value is 5 seconds.
        /// </summary>
        public TimeSpan MDnsDelay { get; set; } = TimeSpan.FromSeconds(5);
        internal bool WriteIndented { get; set; }
    }
}
