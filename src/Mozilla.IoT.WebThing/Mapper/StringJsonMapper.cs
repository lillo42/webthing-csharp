using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class StringJsonMapper : IJsonMapper
    {
        private static StringJsonMapper? s_instance;
        public static StringJsonMapper Instance => s_instance ??= new StringJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetString();
    }
}
