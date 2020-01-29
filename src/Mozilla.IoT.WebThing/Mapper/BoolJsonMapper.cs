using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class BoolJsonMapper : IJsonMapper
    {
        private static BoolJsonMapper? s_instance;
        public static BoolJsonMapper Instance => s_instance ??= new BoolJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetBoolean();
    }
}
