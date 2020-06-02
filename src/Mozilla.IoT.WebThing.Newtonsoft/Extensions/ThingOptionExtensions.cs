using System.Collections.Generic;
using System.Text.Json;
using Mozilla.IoT.WebThing.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mozilla.IoT.WebThing.Newtonsoft
{
    /// <summary>
    /// The extension to <see cref="ThingOption"/>
    /// </summary>
    public static class ThingOptionExtensions
    {
        private static readonly object s_locker = new object();
        private static JsonSerializerSettings? s_serializer = null;
        
        internal static JsonSerializerSettings ToJsonSerializerSettings(this ThingOption option)
        {
            if (s_serializer == null)
            {
                lock (s_locker)
                {
                    if (s_serializer == null)
                    {
                        s_serializer = new JsonSerializerSettings
                        {
                            Formatting = Formatting.None,
                            ContractResolver = option.PropertyNamingPolicy == JsonNamingPolicy.CamelCase
                                ? new CamelCasePropertyNamesContractResolver()
                                : new DefaultContractResolver()
                        };

                        foreach (var converter in s_converters)
                        {
                            s_serializer.Converters.Add(converter);
                        }
                        
                        s_converters.Clear();
                    }
                }
            }

            return s_serializer!;
        }

        private static readonly LinkedList<JsonConverter> s_converters = new LinkedList<JsonConverter>();
        
        /// <summary>
        /// Add <see cref="JsonConverter"/>
        /// </summary>
        /// <param name="option">The <see cref="ThingOption"/>.</param>
        /// <param name="convert">The <see cref="JsonConverter"/> to be added.</param>
        /// <returns>The same instance passed in option.</returns>
        public static ThingOption AddJsonConvert(this ThingOption option, JsonConverter convert)
        {
            s_converters.AddLast(convert);
            return option;
        }
    }
}
