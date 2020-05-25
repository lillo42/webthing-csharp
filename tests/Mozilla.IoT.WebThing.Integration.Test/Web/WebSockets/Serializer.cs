using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.Integration.Test.Web.WebSockets
{
    public class Serializer
    {
        public static ArraySegment<byte> Serialize<T>(T value)
        {
            return new ArraySegment<byte>(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            }));
        }
        
        public static JToken Deserialize(ArraySegment<byte> arraySegment, WebSocketReceiveResult socketReceiveResult)
        {
            var array = new byte[socketReceiveResult.Count];

            Array.Copy(arraySegment.Array, 0, array, 0, array.Length);
            
            return JToken.Parse(Encoding.UTF8.GetString(array));
        }
    }
}
