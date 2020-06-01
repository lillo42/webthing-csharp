using System;
using System.Collections.Generic;
using System.Text;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Newtonsoft
{
    /// <summary>
    /// 
    /// </summary>
    public class NewtonsoftJsonConvert : IJsonConvert
    {
        private readonly JsonSerializerSettings _settings;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        public NewtonsoftJsonConvert(ThingOption option)
        {
            _settings = option?.ToJsonSerializerSettings() ?? throw new ArgumentNullException(nameof(option));
        }

        /// <inheritdoc cref="IJsonConvert"/>
        public T Deserialize<T>(ReadOnlySpan<byte> values)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(values), _settings)!;
        }

        /// <inheritdoc cref="IJsonConvert"/>
        public byte[] Serialize<T>(T value)
        {
            var result = JsonConvert.SerializeObject(value, _settings);
            return Encoding.UTF8.GetBytes(result);
        }

        /// <inheritdoc cref="IJsonConvert"/>
        public IEnumerable<KeyValuePair<string, object>> ToEnumerable(object data)
        {
            if (!(data is JToken token))
            {
                yield break;
            }

            foreach (var kv in token.ToObject<Dictionary<string, object>>()!)
            {
                yield return kv;
            }
        }
    }
}
