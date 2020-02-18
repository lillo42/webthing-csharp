using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class IntJsonMapper : IJsonMapper
    {
        private static IntJsonMapper? s_instance;
        public static IntJsonMapper Instance => s_instance ??= new IntJsonMapper();

        public object Map(object value)
        {
            var element = (JsonElement)value;
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return element.GetInt32();
        }
    }
}
