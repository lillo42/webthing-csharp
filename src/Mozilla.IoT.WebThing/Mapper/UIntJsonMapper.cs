using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class UIntJsonMapper : IJsonMapper
    {
        private static UIntJsonMapper? s_instance;
        public static UIntJsonMapper Instance => s_instance ??= new UIntJsonMapper();

        public object Map(object value)
        {
            var element = (JsonElement)value;
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return element.GetUInt32();
        }
    }
}
