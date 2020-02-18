using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class SByteJsonMapper : IJsonMapper
    {
        private static SByteJsonMapper? s_instance;
        public static SByteJsonMapper Instance => s_instance ??= new SByteJsonMapper();

        public object Map(object value)
        {
            var element = (JsonElement)value;
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return element.GetSByte();
        }
    }
}
