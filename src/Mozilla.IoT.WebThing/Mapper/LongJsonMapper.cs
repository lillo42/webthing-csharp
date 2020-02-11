using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class LongJsonMapper : IJsonMapper
    {
        private static LongJsonMapper? s_instance;
        public static LongJsonMapper Instance => s_instance ??= new LongJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetInt64();
    }
}
