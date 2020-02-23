using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class UShortJsonMapper : IJsonMapper
    {
        private static UShortJsonMapper? s_instance;
        public static UShortJsonMapper Instance => s_instance ??= new UShortJsonMapper();

        public object Map(object value)
        {
            var element = (JsonElement)value;
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return element.GetUInt16();
        }
        
    }
}
