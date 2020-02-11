using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class ByteJsonMapper : IJsonMapper
    {
        private static ByteJsonMapper? s_instance;
        public static ByteJsonMapper Instance => s_instance ??= new ByteJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetByte();
    }
}
