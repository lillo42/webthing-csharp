using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class ShortJsonMapper : IJsonMapper
    {
        private static ShortJsonMapper? s_instance;
        public static ShortJsonMapper Instance => s_instance ??= new ShortJsonMapper();

        public object Map(object value)
        {
            var element = (JsonElement)value;
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return element.GetInt16();
        }
    }
}
