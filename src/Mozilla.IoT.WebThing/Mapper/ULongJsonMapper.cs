using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class ULongJsonMapper : IJsonMapper
    {
        private static ULongJsonMapper s_instance;
        public static ULongJsonMapper Instance => s_instance ??= new ULongJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetUInt64();
    }
}
