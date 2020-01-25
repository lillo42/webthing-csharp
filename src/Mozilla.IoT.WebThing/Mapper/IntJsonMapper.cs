using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class IntJsonMapper : IJsonMapper
    {
        private static IntJsonMapper s_instance;
        public static IntJsonMapper Instance => s_instance ??= new IntJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetInt32();
    }
}
