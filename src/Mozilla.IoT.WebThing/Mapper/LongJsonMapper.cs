using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class LongJsonMapper : IJsonMapper
    {
        private static LongJsonMapper? s_instance;
        public static LongJsonMapper Instance => s_instance ??= new LongJsonMapper();

        public object Map(object value)
        {
            var element = (JsonElement)value;
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return element.GetInt64();
        }
    }
}
